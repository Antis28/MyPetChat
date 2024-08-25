using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChatClientWPF.Models
{
    public class ClientObject : INotifyPropertyChanged
    {

        public ClientObject(string name)
        {
            UserName = name;
            Id = "-1";
            IPAddress = "127.0.0.1";
        }

        public string Id { get; set; }
        private string _name;
        public string UserName
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        public string IPAddress { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
