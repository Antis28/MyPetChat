using System.Windows;
using System.Windows.Input;
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
       

        void ConnectUser()
        {
            if (!isConnected)
            {
                btnConDiscon.Content = "Отключить";
                isConnected = true;

                client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                ID = client.Connect(tbUserName.Text);
                tbUserName.IsEnabled = false;
               
                
            }
        }
        void DisconnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(ID);
                client = null;
                tbUserName.IsEnabled = true;
                btnConDiscon.Content = "Подключить";
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

        private void tbMessege_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client == null) return;
               
                client.SendMsg(tbMessege.Text, ID);
                tbMessege.Text = string.Empty;
            }
        }

    }
}
