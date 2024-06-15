using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm.CodeGenerators;

namespace ChatClientWPF.ViewModels
{
    [GenerateViewModel]
    public partial class MainViewModel 
    {
        [GenerateProperty]
        string userName;
        [GenerateProperty]
        public string ip;
        [GenerateProperty]
        public string port;
        [GenerateProperty]
        string status;

        [GenerateCommand]
        void Login() => Status = "User: " + userName;
        bool CanLogin() => !string.IsNullOrEmpty(userName);
        void ConnectCommand() => Status = "";
    }
}
