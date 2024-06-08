﻿using System.Collections.Generic;
using System.Linq;
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
        List<ClientUser> clientUsers = new List<ClientUser>();
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

                clientUsers = new List<ClientUser>(client.GetUsers());

                UpdateUserList(clientUsers);

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
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
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
        void UpdateUserList(List<ClientUser> clientUsers)
        {
            lbUsers.Items.Clear();
            foreach (var item in clientUsers)
            {
                lbUsers.Items.Add(item.Name);
            }

        }

        public void ArrivedUserCallBack(ClientUser user)
        {
            clientUsers.Add(user);
            var userSerch = clientUsers.FirstOrDefault(x => x.ID == user.ID);
            if (userSerch == null) return;

            UpdateUserList(clientUsers);
        }

        public void GoneUserCallBack(ClientUser user)
        {
            var userSerch = clientUsers.FirstOrDefault(x => x.ID == user.ID);
            if (userSerch == null) return;

            clientUsers.Remove(user);
            UpdateUserList(clientUsers);
        }

        public void UserListUpdatedCallBack(ClientUser[] users)
        {
            UpdateUserList(new List<ClientUser>(users));

        }
    }
}
