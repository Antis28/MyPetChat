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

namespace ChatClientWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isConnected;
        public MainWindow()
        {
            InitializeComponent();
        }

        void ConnectUser()
        {
            if (!isConnected)
            {
                tbUserName.IsEnabled = false;
                btnConDiscon.Content = "Disconnect";
                isConnected = true;
            }
        }
        void DisconnectUser()
        {
            if (isConnected)
            {
                tbUserName.IsEnabled = true;
                btnConDiscon.Content = "Connect";
                isConnected = false;
            }
        }

        private void btnConDiscon_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected) DisconnectUser();
            else ConnectUser();
        }
    }
}
