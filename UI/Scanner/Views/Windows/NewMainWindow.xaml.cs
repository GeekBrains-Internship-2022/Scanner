using Scanner.Infrastructure.Commands;
using Scanner.Models;
using System;
using System.Collections.Generic;
using System.Data.MoonPdf.Wpf;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Scanner.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewMainWindow.xaml
    /// </summary>
    public partial class NewMainWindow : Window
    {
        public NewMainWindow()
        {
            InitializeComponent();
            this.ListBoxinOP.SelectionChanged += ListBoxInopSelectionChanged;

        }

        private void ListBoxInopSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.MoonPdfPanelINOP.Unload();
            if (e.AddedItems[0] is FileData data)
            {
                if (data is null || data.FilePath is null)
                    return;
                if (!File.Exists(data.FilePath)) return;

                try
                {
                    this.MoonPdfPanelINOP.OpenFile(data.FilePath);
                    this.MoonPdfPanelINOP.PageRowDisplay = PageRowDisplayType.ContinuousPageRows;
                }
                catch (Exception)
                {
                    return;
                }
            }
            
            //this.MoonPdfPanelINOP
        }

        #region Команды

        #region ZoomInCommand - команда увеличить

        private ICommand _ZoomInCommand;

        public ICommand ZoomInCommand => _ZoomInCommand
            ??= new LambdaCommand(OnZoomInCommandExecuted, CanZoomInCommandExecute);

        private void OnZoomInCommandExecuted(object p) => this.MoonPdfPanelINOP.ZoomIn();

        private bool CanZoomInCommandExecute(object p) => this.ListBoxinOP.SelectedItem is not null;

        #endregion

        #region ZoomOutCommand - команда уменьшить

        private ICommand _ZoomOutCommand;

        public ICommand ZoomOutCommand => _ZoomOutCommand
            ??= new LambdaCommand(OnZoomOutCommandExecuted, CanZoomOutCommandExecute);

        private void OnZoomOutCommandExecuted(object p) => this.MoonPdfPanelINOP.ZoomOut();

        private bool CanZoomOutCommandExecute(object p) => this.ListBoxinOP.SelectedItem is not null;

        #endregion

        #region ZoomResetCommand - команда сброса масштаба

        private ICommand _ZoomResetCommand;

        public ICommand ZoomResetCommand => _ZoomResetCommand
            ??= new LambdaCommand(OnZoomResetCommandExecuted, CanZoomResetCommandExecute);

        private void OnZoomResetCommandExecuted(object p) => this.MoonPdfPanelINOP.Zoom(1d);

        private bool CanZoomResetCommandExecute(object p) => this.ListBoxinOP.SelectedItem is not null;

        #endregion

        #region ZoomToHeightCommand - команда масштаб по высоте

        private ICommand _ZoomToHeightCommand;

        public ICommand ZoomToHeightCommand => _ZoomToHeightCommand
            ??= new LambdaCommand(OnZoomToHeightCommandExecuted, CanZoomToHeightCommandExecute);

        private void OnZoomToHeightCommandExecuted(object p) => this.MoonPdfPanelINOP.ZoomToHeight();

        private bool CanZoomToHeightCommandExecute(object p) => this.ListBoxinOP.SelectedItem is not null;

        #endregion

        #region ZoomToWidthCommand - команда масштаб по ширине

        private ICommand _ZoomToWidthCommand;

        public ICommand ZoomToWidthCommand => _ZoomToWidthCommand
            ??= new LambdaCommand(OnZoomToWidthCommandExecuted, CanZoomToWidthCommandExecute);

        private void OnZoomToWidthCommandExecuted(object p) => this.MoonPdfPanelINOP.ZoomToWidth();

        private bool CanZoomToWidthCommandExecute(object p) => this.ListBoxinOP.SelectedItem is not null;

        #endregion

        #region SinglePageViewCommand - команда одностраничного отображения

        private ICommand _SinglePageViewCommand;

        public ICommand SinglePageViewCommand => _SinglePageViewCommand
            ??= new LambdaCommand(OnSinglePageViewCommandExecuted, CanSinglePageViewCommandExecute);

        private void OnSinglePageViewCommandExecuted(object p) => this.MoonPdfPanelINOP.ViewType = ViewType.SinglePage;

        private bool CanSinglePageViewCommandExecute(object p) => this.ListBoxinOP.SelectedItem is not null;

        #endregion

        #region DoublePageViewCommand - команда двухстраничного отображения

        private ICommand _DoublePageViewCommand;

        public ICommand DoublePageViewCommand => _DoublePageViewCommand
            ??= new LambdaCommand(OnDoublePageViewCommandExecuted, CanDoublePageViewCommandExecute);

        private void OnDoublePageViewCommandExecuted(object p) => this.MoonPdfPanelINOP.ViewType = ViewType.Facing;

        private bool CanDoublePageViewCommandExecute(object p) => this.ListBoxinOP.SelectedItem is not null;

        #endregion

        #endregion
    }
}
