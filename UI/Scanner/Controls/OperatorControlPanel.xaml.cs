﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Scanner.Models;
using Scanner.ViewModels.Models;

namespace Scanner.Controls
{
    public partial class OperatorControlPanel : UserControl
    {
        #region TemplatesItemSource : ObservableCollection<ScannerDataTemplate> - Шаблоны

        ///<summary>Шаблоны</summary>
        public static readonly DependencyProperty TemplatesItemSourceProperty =
            DependencyProperty.Register(
                nameof(TemplatesItemSource),
                typeof(ObservableCollection<ScannerDataTemplate>),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(ObservableCollection<ScannerDataTemplate>)));

        ///<summary>Шаблоны</summary>
        public ObservableCollection<ScannerDataTemplate> TemplatesItemSource
        {
            get => (ObservableCollection<ScannerDataTemplate>) GetValue(TemplatesItemSourceProperty);
            set => SetValue(TemplatesItemSourceProperty, value);
        }

        #endregion

        #region SelectedTemplate : ScannerDataTemplate - Выбранный шаблон

        ///<summary>Выбранный шаблон</summary>
        public static readonly DependencyProperty SelectedTemplateProperty =
            DependencyProperty.Register(
                nameof(SelectedTemplate),
                typeof(ScannerDataTemplate),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(ScannerDataTemplate)));

        ///<summary>Выбранный шаблон</summary>
        public ScannerDataTemplate SelectedTemplate
        {
            get => (ScannerDataTemplate) GetValue(SelectedTemplateProperty);
            set => SetValue(SelectedTemplateProperty, value);
        }

        #endregion

        #region MetadataItemSource : ObservableCollection<DocumentMetadata> - Метаданные

        ///<summary>Метаданные</summary>
        public static readonly DependencyProperty MetadataItemSourceProperty =
            DependencyProperty.Register(
                nameof(MetadataItemSource),
                typeof(ObservableCollection<DocumentMetadata>),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(ObservableCollection<DocumentMetadata>)));

        ///<summary>Метаданные</summary>
        public ObservableCollection<DocumentMetadata> MetadataItemSource
        {
            get => (ObservableCollection<DocumentMetadata>) GetValue(MetadataItemSourceProperty);
            set => SetValue(MetadataItemSourceProperty, value);
        }

        #endregion

        #region SelectedMetadata : DocumentMetadata - Выбранные метаданные

        ///<summary>Выбранные метаданные</summary>
        public static readonly DependencyProperty SelectedMetadataProperty =
            DependencyProperty.Register(
                nameof(SelectedMetadata),
                typeof(DocumentMetadata),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(DocumentMetadata)));

        ///<summary>Выбранные метаданные</summary>
        public DocumentMetadata SelectedMetadata
        {
            get => (DocumentMetadata) GetValue(SelectedMetadataProperty);
            set => SetValue(SelectedMetadataProperty, value);
        }

        #endregion

        #region CBItemSource : ObservableCollection<string> - коллекция метаданных в выбранном шаблоне

        ///<summary>коллекция подпапок</summary>
        public static readonly DependencyProperty CBItemSourceProperty =
            DependencyProperty.Register(
                nameof(CBItemSource),
                typeof(ObservableCollection<string>),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(ObservableCollection<string>)));

        ///<summary>коллекция подпапок</summary>
        public ObservableCollection<string> CBItemSource
        {
            get => (ObservableCollection<string>)GetValue(CBItemSourceProperty);
            set => SetValue(CBItemSourceProperty, value);
        }

        #endregion

        #region CBSelectedItem : string - выбранный тип метаданного

        ///<summary>выбранный объект</summary>
        public static readonly DependencyProperty CBSelectedItemProperty =
            DependencyProperty.Register(
                nameof(CBSelectedItem),
                typeof(string),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(string)));

        ///<summary>выбранный объект</summary>
        public string CBSelectedItem
        {
            get => (string)GetValue(CBSelectedItemProperty);
            set => SetValue(CBSelectedItemProperty, value);
        }

        #endregion

        #region SaveCommand : ICommand - Команда сохранить

        ///<summary>Команда сохранить</summary>
        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register(
                nameof(SaveCommand),
                typeof(ICommand),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(ICommand)));

        ///<summary>Команда сохранить</summary>
        public ICommand SaveCommand
        {
            get => (ICommand) GetValue(SaveCommandProperty);
            set => SetValue(SaveCommandProperty, value);
        }

        #endregion

        #region NextFileCommand : ICommand - Команда следующий документ

        ///<summary>Команда следующий документ</summary>
        public static readonly DependencyProperty NextFileCommandProperty =
            DependencyProperty.Register(
                nameof(NextFileCommand),
                typeof(ICommand),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(ICommand)));

        ///<summary>Команда следующий документ</summary>
        public ICommand NextFileCommand
        {
            get => (ICommand) GetValue(NextFileCommandProperty);
            set => SetValue(NextFileCommandProperty, value);
        }

        #endregion

        public OperatorControlPanel() => InitializeComponent();
    }
}
