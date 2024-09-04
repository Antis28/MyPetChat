using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
