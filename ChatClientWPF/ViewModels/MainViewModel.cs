using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm.CodeGenerators;

namespace ChatClientWPF
{
    [GenerateViewModel]
    public partial class MainViewModel 
    {
        [GenerateProperty]
        string username;
        [GenerateProperty]
        public string ip;
        [GenerateProperty]
        string status;

        [GenerateCommand]
        void Login() => Status = "User: " + Username;
        bool CanLogin() => !string.IsNullOrEmpty(Username);
    }
}
