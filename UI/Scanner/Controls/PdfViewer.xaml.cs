using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.MoonPdf.Wpf;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Scanner.Infrastructure.Commands;
using Scanner.Models;

namespace Scanner.Controls
{
    public partial class PdfViewer : UserControl, INotifyPropertyChanged
    {
        #region ItemSource : ObservableCollection<ScanDocument> - коллекция документов

        ///<summary>выбранный документ</summary>
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register(
                nameof(ItemSource),
                typeof(ObservableCollection<ScanDocument>),
                typeof(PdfViewer),
                new PropertyMetadata(default(ObservableCollection<ScanDocument>)));

        ///<summary>выбранный документ</summary>
        public ObservableCollection<ScanDocument> ItemSource
        {
            get => (ObservableCollection<ScanDocument>)GetValue(ItemSourceProperty);
            set => SetValue(ItemSourceProperty, value);
        }

        #endregion

        #region CBItemSource : ObservableCollection<string> - коллекция подпапок

        ///<summary>коллекция подпапок</summary>
        public static readonly DependencyProperty CBItemSourceProperty =
            DependencyProperty.Register(
                nameof(CBItemSource),
                typeof(ObservableCollection<string>),
                typeof(PdfViewer),
                new PropertyMetadata(default(ObservableCollection<string>)));

        ///<summary>коллекция подпапок</summary>
        public ObservableCollection<string> CBItemSource
        {
            get => (ObservableCollection<string>)GetValue(CBItemSourceProperty);
            set => SetValue(CBItemSourceProperty, value);
        }

        #endregion

        #region SelectedItem : ScanDocument - выбранный объект

        ///<summary>выбранный объект</summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(ScanDocument),
                typeof(PdfViewer),
                new PropertyMetadata(default(ScanDocument)));

        ///<summary>выбранный объект</summary>
        public ScanDocument SelectedItem
        {
            get => (ScanDocument)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        #endregion

        #region CBSelectedItem : string - выбранный тип документа

        ///<summary>выбранный объект</summary>
        public static readonly DependencyProperty CBSelectedItemProperty =
            DependencyProperty.Register(
                nameof(CBSelectedItem),
                typeof(string),
                typeof(PdfViewer),
                new PropertyMetadata(default(string)));

        ///<summary>выбранный объект</summary>
        public string CBSelectedItem
        {
            get => (string)GetValue(CBSelectedItemProperty);
            set => SetValue(CBSelectedItemProperty, value);
        }

        #endregion

        #region InternalSelectedItem : ScanDocument - выбранный объект
        //  TODO: решить вопрос с костылями
        private ScanDocument _InternalSelectedItem;

        public ScanDocument InternalSelectedItem
        {
            get => _InternalSelectedItem;
            set
            {
                Set(ref _InternalSelectedItem, value);
                SelectedItem = value;
                PdfView(value);
            }
        }

        #endregion

        private void PdfView(ScanDocument document)
        {
            if (document is null)
                return;

            try
            {
                MoonPdfPanel.OpenFile(document.Path);
                MoonPdfPanel.PageRowDisplay = PageRowDisplayType.ContinuousPageRows;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PdfViewer() => InitializeComponent();

        #region Команды

        #region ZoomInCommand - команда увеличить

        private ICommand _ZoomInCommand;

        public ICommand ZoomInCommand => _ZoomInCommand
            ??= new LambdaCommand(OnZoomInCommandExecuted, CanZoomInCommandExecute);

        private void OnZoomInCommandExecuted(object p) => MoonPdfPanel.ZoomIn();

        private bool CanZoomInCommandExecute(object p) => InternalSelectedItem is not null;

        #endregion

        #region ZoomOutCommand - команда уменьшить

        private ICommand _ZoomOutCommand;

        public ICommand ZoomOutCommand => _ZoomOutCommand
            ??= new LambdaCommand(OnZoomOutCommandExecuted, CanZoomOutCommandExecute);

        private void OnZoomOutCommandExecuted(object p) => MoonPdfPanel.ZoomOut();

        private bool CanZoomOutCommandExecute(object p) => InternalSelectedItem is not null;

        #endregion

        #region ZoomResetCommand - команда сброса масштаба

        private ICommand _ZoomResetCommand;

        public ICommand ZoomResetCommand => _ZoomResetCommand
            ??= new LambdaCommand(OnZoomResetCommandExecuted, CanZoomResetCommandExecute);

        private void OnZoomResetCommandExecuted(object p) => MoonPdfPanel.Zoom(1d);

        private bool CanZoomResetCommandExecute(object p) => InternalSelectedItem is not null;

        #endregion

        #region ZoomToHeightCommand - команда масштаб по высоте

        private ICommand _ZoomToHeightCommand;

        public ICommand ZoomToHeightCommand => _ZoomToHeightCommand
            ??= new LambdaCommand(OnZoomToHeightCommandExecuted, CanZoomToHeightCommandExecute);

        private void OnZoomToHeightCommandExecuted(object p) => MoonPdfPanel.ZoomToHeight();

        private bool CanZoomToHeightCommandExecute(object p) => InternalSelectedItem is not null;

        #endregion

        #region ZoomToWidthCommand - команда масштаб по ширине

        private ICommand _ZoomToWidthCommand;

        public ICommand ZoomToWidthCommand => _ZoomToWidthCommand
            ??= new LambdaCommand(OnZoomToWidthCommandExecuted, CanZoomToWidthCommandExecute);

        private void OnZoomToWidthCommandExecuted(object p) => MoonPdfPanel.ZoomToWidth();

        private bool CanZoomToWidthCommandExecute(object p) => InternalSelectedItem is not null;

        #endregion

        #region SinglePageViewCommand - команда одностраничного отображения

        private ICommand _SinglePageViewCommand;

        public ICommand SinglePageViewCommand => _SinglePageViewCommand
            ??= new LambdaCommand(OnSinglePageViewCommandExecuted, CanSinglePageViewCommandExecute);

        private void OnSinglePageViewCommandExecuted(object p) => MoonPdfPanel.ViewType = ViewType.SinglePage;

        private bool CanSinglePageViewCommandExecute(object p) => InternalSelectedItem is not null;

        #endregion

        #region DoublePageViewCommand - команда двухстраничного отображения

        private ICommand _DoublePageViewCommand;

        public ICommand DoublePageViewCommand => _DoublePageViewCommand
            ??= new LambdaCommand(OnDoublePageViewCommandExecuted, CanDoublePageViewCommandExecute);

        private void OnDoublePageViewCommandExecuted(object p) => MoonPdfPanel.ViewType = ViewType.Facing;

        private bool CanDoublePageViewCommandExecute(object p) => InternalSelectedItem is not null;

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
