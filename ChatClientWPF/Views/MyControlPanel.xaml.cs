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

namespace ChatClientWPF.Views
{
    /// <summary>
    /// Логика взаимодействия для MyControlPanel.xaml
    /// </summary>
    public partial class MyControlPanel : UserControl
    {
        public MyControlPanel()
        {
            InitializeComponent();
        }
        private void btnConDiscon_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    if (isConnected) DisconnectUser();
            //    else ConnectUser();
            //}
            //catch (System.Exception ex)
            //{
            //    lbChat.Items.Add(ex.Message);
            //}

        }
    }
}
