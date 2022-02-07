﻿using Cube.Pdf.Pdfium;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Scanner.interfaces;
using Scanner.interfaces.RabbitMQ;
using Scanner.Models;
using Scanner.Service;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Scanner.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            PdfViewTest();
        }

        #region For test without ui

        private static readonly IConfiguration __Configuration = App.Services.GetRequiredService<IConfiguration>();

        private static void MethodForTest()
        {
            //ObserverTest();
            //FileServiceTest();
            //RabbitTest();
        }

        private static void RabbitTest()
        {
            var rabbit = App.Services.GetRequiredService<IRabbitMQService>();
            var doc = new Document
            {
                DocumentType = "Test Document",
                IndexingDate = DateTime.Now,
                Metadata = Enumerable
                    .Range(1, 5)
                    .Select(i => new DocumentMetadata
                    {
                        Id = i,
                        Data = $"Data-{i}",
                        Name = $"Name-{i}"
                    }).ToArray()
            };

            rabbit.Publish(doc);
        }

        private static void ObserverTest()
        {
            var observer = App.Services.GetRequiredService<ObserverService>();
            var path = __Configuration["ObserverPath"];
            var task = new Task(() => observer.Start(path));

            observer.Notify += OnNotify;
            task.Start();

            void OnNotify(string message) => Debug.WriteLine(message);
        }

        private static void FileServiceTest()
        {
            var filePath = @"D:\111\111.pdf";
            var fileService = App.Services.GetRequiredService<IFileService>();
            var fileData = fileService.CreateFileData(filePath, "Pasport");

            fileService.Move(filePath, fileName: fileData.Guid.ToString());
        }

        private void PdfViewTest()
        {
            string tempfilePath = "..\\..\\..\\Образец.pdf";

            moonPdfPanel.OpenFile(tempfilePath);
            moonPdfPanel.PageRowDisplay = System.Data.MoonPdf.Wpf.PageRowDisplayType.ContinuousPageRows;
            //System.Drawing.Image imgPage;
            //DocumentReader documentReader;

            //using (DocumentRenderer documentRenderer = new DocumentRenderer(tempfilePath))
            //{
            //    documentReader = new DocumentReader(tempfilePath);
            //    var pages = documentReader.Pages.ToArray();
            //    imgPage = documentRenderer.Render(pages[0], pages[0].Size);

            //    Bitmap bitmap = new Bitmap(imgPage);
            //    BitmapSource bmpResource = getBitMapSourceFromBitmap(bitmap);
            //    image.Source = bmpResource;
            //}
        }

        #endregion

        private void mniSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void mniExit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        /// <summary>
        /// Bitmap->BitmapSource
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource getBitMapSourceFromBitmap(Bitmap bitmap)
        {
            IntPtr intPtrl = bitmap.GetHbitmap();
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(intPtrl,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(intPtrl);
            return bitmapSource;
        }

        public byte[] ConvertBitMapToByteArray(Bitmap bitmap)
        {
            byte[] result = null;

            if (bitmap != null)
            {
                ImageConverter converter = new ImageConverter();
                result = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));

                //MemoryStream stream = new MemoryStream();
                //bitmap.Save(stream, bitmap.RawFormat);
                //result = stream.ToArray();
            }

            return result;
        }

    }
}
