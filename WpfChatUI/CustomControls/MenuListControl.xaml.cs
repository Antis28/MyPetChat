using System.Windows.Controls;

namespace WpfChatUI.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для MenuListControl.xaml
    /// </summary>
    public partial class MenuListControl : UserControl
    {
        public MenuListControl()
        {
            InitializeComponent();
            //We are going to bind our MenuItems to the CustomList
            DataContext = new ViewModel();
        }
    }
}
