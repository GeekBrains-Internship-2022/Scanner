using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Scanner.Models;

namespace Scanner.Controls
{
    /// <summary>
    /// Логика взаимодействия для OperatorControlPanel.xaml
    /// </summary>
    public partial class OperatorControlPanel : UserControl
    {
        #region ItemSource : ObservableCollection<Template> - Шаблоны

        ///<summary>Шаблоны</summary>
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register(
                nameof(ItemSource),
                typeof(ObservableCollection<Template>),
                typeof(OperatorControlPanel),
                new PropertyMetadata(default(ObservableCollection<Template>)));

        ///<summary>Шаблоны</summary>
        public ObservableCollection<Template> ItemSource
        {
            get => (ObservableCollection<Template>) GetValue(ItemSourceProperty);
            set => SetValue(ItemSourceProperty, value);
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

        public OperatorControlPanel() => InitializeComponent();


    }
}
