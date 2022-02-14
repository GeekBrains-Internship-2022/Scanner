using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Scanner.Models;
using Scanner.ViewModels.Models;

namespace Scanner.Controls
{
    public partial class OperatorControlPanel : UserControl
    {
        #region TemplatesItemSource : ObservableCollection<Template> - Шаблоны

        ///<summary>Шаблоны</summary>
        public static readonly DependencyProperty TemplatesItemSourceProperty =
            DependencyProperty.Register(
                nameof(TemplatesItemSource),
                typeof(ObservableCollection<Template>),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(ObservableCollection<Template>)));

        ///<summary>Шаблоны</summary>
        public ObservableCollection<Template> TemplatesItemSource
        {
            get => (ObservableCollection<Template>) GetValue(TemplatesItemSourceProperty);
            set => SetValue(TemplatesItemSourceProperty, value);
        }

        #endregion

        #region SelectedTemplate : Template - Выбранный шаблон

        ///<summary>Выбранный шаблон</summary>
        public static readonly DependencyProperty SelectedTemplateProperty =
            DependencyProperty.Register(
                nameof(SelectedTemplate),
                typeof(Template),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(Template)));

        ///<summary>Выбранный шаблон</summary>
        public Template SelectedTemplate
        {
            get => (Template) GetValue(SelectedTemplateProperty);
            set => SetValue(SelectedTemplateProperty, value);
        }

        #endregion

        #region MetadataItemSource : ObservableCollection<Metadata> - Метаданные

        ///<summary>Метаданные</summary>
        public static readonly DependencyProperty MetadataItemSourceProperty =
            DependencyProperty.Register(
                nameof(MetadataItemSource),
                typeof(ObservableCollection<Metadata>),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(ObservableCollection<Metadata>)));

        ///<summary>Метаданные</summary>
        public ObservableCollection<Metadata> MetadataItemSource
        {
            get => (ObservableCollection<Metadata>) GetValue(MetadataItemSourceProperty);
            set => SetValue(MetadataItemSourceProperty, value);
        }

        #endregion

        #region SelectedMetadata : Metadata - Выбранные метаданные

        ///<summary>Выбранные метаданные</summary>
        public static readonly DependencyProperty SelectedMetadataProperty =
            DependencyProperty.Register(
                nameof(SelectedMetadata),
                typeof(Metadata),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(Metadata)));

        ///<summary>Выбранные метаданные</summary>
        public Metadata SelectedMetadata
        {
            get => (Metadata) GetValue(SelectedMetadataProperty);
            set => SetValue(SelectedMetadataProperty, value);
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
