using DevExpress.Mvvm.CodeGenerators;
using System;
using System.Collections.ObjectModel;

namespace ChatClientWPF.ViewModels
{
    [GenerateViewModel]
    public partial class MainViewModel //: ViewModelBase
    {
        [GenerateProperty]
        string userName;
        [GenerateProperty]
        public string ip;
        [GenerateProperty]
        public string port;
        [GenerateProperty]
        string status;

        [GenerateProperty]
        ObservableCollection<string> chat;
       
        [GenerateProperty]
        ObservableCollection<string> userNames;
        
        [GenerateProperty]
        string message;

        [GenerateCommand]
        void Login() => Status = "User: " + userName;
        bool CanLogin() => !string.IsNullOrEmpty(userName);
        void ConnectCommand() => Status = "";

        public MainViewModel()
        {
            ip = "192.168.1.105";
            port = "5050";

            userName = RandomeUserName();
        }

        private string RandomeUserName()
        {
            var names = new string[] { "Biser", "Tiser", "Ruser", "Niser", "Miser", "Cuser", "User", "Diser" };
            var r = new Random((int)DateTime.Now.Ticks);
            return names[r.Next(names.Length)];
        }
    }
}
