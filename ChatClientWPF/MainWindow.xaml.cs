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
using ChatClientWPF.ServiceChat;

namespace ChatClientWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IServiceChatCallback
    {
        bool isConnected;
        ServiceChatClient client;
        int ID;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
        }

        void ConnectUser()
        {
            if (!isConnected)
            {
                ID = client.Connect(tbUserName.Text);
                tbUserName.IsEnabled = false;
                btnConDiscon.Content = "Disconnect";
                isConnected = true;
            }
        }
        void DisconnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(ID);
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

        public void MsgCallBack(string msg)
        {
            lbChat.Items.Add(msg);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }
    }
}
