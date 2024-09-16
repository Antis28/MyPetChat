using ChatClientWPF.ServiceChat;
using ChatClientWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.ServiceModel;
using System.Windows;

namespace ChatClientWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ClientUser> clientUsers = new List<ClientUser>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.DisconnectCommandCommand.Execute(this);
                e.Cancel = false; // Не отменяем закрытие окна
            }
        }

        /// <summary>
        /// полученный
        /// </summary>
        /// <param name="user"></param>
        public void ArrivedUserCallBack(ClientUser user)
        {
            clientUsers.Add(user);
            var userSerch = clientUsers.FirstOrDefault(x => x.ID == user.ID);
            if (userSerch == null) { return; }

            UpdateUserList(clientUsers);
        }

        private void UpdateUserList(List<ClientUser> clientUsers)
        {
            throw new NotImplementedException();
        }

        public void GoneUserCallBack(ClientUser user)
        {
            var userSerch = clientUsers.FirstOrDefault(x => x.ID == user.ID);
            if (userSerch == null) { return; }

            clientUsers.Remove(user);
            UpdateUserList(clientUsers);
        }
    }
}
