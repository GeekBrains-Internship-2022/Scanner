using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Scanner.Models;
using Scanner.ViewModels.Models;

namespace Scanner.Controls
{
    /// <summary>
    /// Логика взаимодействия для AdministratorControlPanel.xaml
    /// </summary>
    public partial class AdministratorControlPanel : UserControl
    {
        #region DocumentType : string - Тип документа

        ///<summary>Тип документа</summary>
        public static readonly DependencyProperty DocumentTypeProperty =
            DependencyProperty.Register(
                nameof(DocumentType),
                typeof(string),
                typeof(AdministratorControlPanel),
                new PropertyMetadata(default(string)));

        ///<summary>Тип документа</summary>
        public string DocumentType
        {
            get => (string) GetValue(DocumentTypeProperty);
            set => SetValue(DocumentTypeProperty, value);
        }

        #endregion

        #region ItemSource : FileData - Источник

        ///<summary>источник метаданных</summary>
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register(
                nameof(ItemSource),
                typeof(FileData),
                typeof(AdministratorControlPanel),
                new PropertyMetadata(default(FileData)));

        ///<summary>источник метаданных</summary>
        public FileData ItemSource
        {
            get => (FileData) GetValue(ItemSourceProperty);
            set => SetValue(ItemSourceProperty, value);
        }

        #endregion

        #region Metadata : ObservableCollection<TemplateMetadata> - Метаданные

        ///<summary>Метаданные</summary>
        public static readonly DependencyProperty MetadataProperty =
            DependencyProperty.Register(
                nameof(Metadata),
                typeof(ObservableCollection<TemplateMetadata>),
                typeof(AdministratorControlPanel),
                new PropertyMetadata(default(ObservableCollection<TemplateMetadata>)));

        ///<summary>Метаданные</summary>
        public ObservableCollection<TemplateMetadata> Metadata
        {
            get => (ObservableCollection<TemplateMetadata>) GetValue(MetadataProperty);
            set => SetValue(MetadataProperty, value);
        }

        #endregion

        #region FinishCommand : ICommand - Команда завершить обработку

        ///<summary>Команда завершить обработку</summary>
        public static readonly DependencyProperty FinishCommandProperty =
            DependencyProperty.Register(
                nameof(FinishCommand),
                typeof(ICommand),
                typeof(AdministratorControlPanel),
                new PropertyMetadata(default(ICommand)));

        ///<summary>Команда завершить обработку</summary>
        public ICommand FinishCommand
        {
            get => (ICommand) GetValue(FinishCommandProperty);
            set => SetValue(FinishCommandProperty, value);
        }

        #endregion

        #region ReworkCommand : ICommand - Команда доработки

        ///<summary>Команда доработки</summary>
        public static readonly DependencyProperty ReworkCommandProperty =
            DependencyProperty.Register(
                nameof(ReworkCommand),
                typeof(ICommand),
                typeof(AdministratorControlPanel),
                new PropertyMetadata(default(ICommand)));

        ///<summary>Команда доработки</summary>
        public ICommand ReworkCommand
        {
            get => (ICommand) GetValue(ReworkCommandProperty);
            set => SetValue(ReworkCommandProperty, value);
        }

        #endregion

        public AdministratorControlPanel() => InitializeComponent();
    }
}
