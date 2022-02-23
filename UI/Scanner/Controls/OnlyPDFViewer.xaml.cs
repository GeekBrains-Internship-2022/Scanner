using Scanner.Infrastructure.Commands;
using Scanner.Models;
using Scanner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.MoonPdf.Wpf;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scanner.Controls
{
    /// <summary>
    /// Логика взаимодействия для OnlyPDFViewer.xaml
    /// </summary>
    public partial class OnlyPDFViewer : UserControl, INotifyPropertyChanged
    {
        public OnlyPDFViewer()
        {
            InitializeComponent();
            //ItemSource.PropertyChanged += ItemSourcePropertyChanged;
        }

        private void ItemSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            return;
        }

        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register(
                nameof(ItemSource),
                typeof(FileData),
                typeof(OnlyPDFViewer),
                new PropertyMetadata(default(FileData)));
        
        public FileData ItemSource
        {
            get => (FileData)GetValue(ItemSourceProperty);
            set
            {
                SetValue(ItemSourceProperty, value);                
                PdfView(value);
            }
        }

        private void PdfView(FileData document)
        {
            if (document is null)
                return;

            try
            {
                MoonPdfPanel.OpenFile(document.FilePath);
                MoonPdfPanel.PageRowDisplay = PageRowDisplayType.ContinuousPageRows;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Команды

        #region ZoomInCommand - команда увеличить

        private ICommand _ZoomInCommand;

        public ICommand ZoomInCommand => _ZoomInCommand
            ??= new LambdaCommand(OnZoomInCommandExecuted, CanZoomInCommandExecute);

        private void OnZoomInCommandExecuted(object p) => MoonPdfPanel.ZoomIn();

        private bool CanZoomInCommandExecute(object p) => ItemSource is not null;

        #endregion

        #region ZoomOutCommand - команда уменьшить

        private ICommand _ZoomOutCommand;

        public ICommand ZoomOutCommand => _ZoomOutCommand
            ??= new LambdaCommand(OnZoomOutCommandExecuted, CanZoomOutCommandExecute);

        private void OnZoomOutCommandExecuted(object p) => MoonPdfPanel.ZoomOut();

        private bool CanZoomOutCommandExecute(object p) => ItemSource is not null;

        #endregion

        #region ZoomResetCommand - команда сброса масштаба

        private ICommand _ZoomResetCommand;

        public ICommand ZoomResetCommand => _ZoomResetCommand
            ??= new LambdaCommand(OnZoomResetCommandExecuted, CanZoomResetCommandExecute);

        private void OnZoomResetCommandExecuted(object p) => MoonPdfPanel.Zoom(1d);

        private bool CanZoomResetCommandExecute(object p) => ItemSource is not null;

        #endregion

        #region ZoomToHeightCommand - команда масштаб по высоте

        private ICommand _ZoomToHeightCommand;

        public ICommand ZoomToHeightCommand => _ZoomToHeightCommand
            ??= new LambdaCommand(OnZoomToHeightCommandExecuted, CanZoomToHeightCommandExecute);

        private void OnZoomToHeightCommandExecuted(object p) => MoonPdfPanel.ZoomToHeight();

        private bool CanZoomToHeightCommandExecute(object p) => ItemSource is not null;

        #endregion

        #region ZoomToWidthCommand - команда масштаб по ширине

        private ICommand _ZoomToWidthCommand;

        public ICommand ZoomToWidthCommand => _ZoomToWidthCommand
            ??= new LambdaCommand(OnZoomToWidthCommandExecuted, CanZoomToWidthCommandExecute);

        private void OnZoomToWidthCommandExecuted(object p) => MoonPdfPanel.ZoomToWidth();

        private bool CanZoomToWidthCommandExecute(object p) => ItemSource is not null;

        #endregion

        #region SinglePageViewCommand - команда одностраничного отображения

        private ICommand _SinglePageViewCommand;

        public ICommand SinglePageViewCommand => _SinglePageViewCommand
            ??= new LambdaCommand(OnSinglePageViewCommandExecuted, CanSinglePageViewCommandExecute);

        private void OnSinglePageViewCommandExecuted(object p) => MoonPdfPanel.ViewType = ViewType.SinglePage;

        private bool CanSinglePageViewCommandExecute(object p) => ItemSource is not null;

        #endregion

        #region DoublePageViewCommand - команда двухстраничного отображения

        private ICommand _DoublePageViewCommand;

        public ICommand DoublePageViewCommand => _DoublePageViewCommand
            ??= new LambdaCommand(OnDoublePageViewCommandExecuted, CanDoublePageViewCommandExecute);

        private void OnDoublePageViewCommandExecuted(object p) => MoonPdfPanel.ViewType = ViewType.Facing;

        private bool CanDoublePageViewCommandExecute(object p) => ItemSource is not null;

        #endregion

        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string PropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        private bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
    }
}
