using CommonLibrary;
using CommonLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientWPF.ViewModels
{
    internal class WpfLogger : ILogger
    {
        ObservableCollection<CommandMessage> _chat;
        Action<string> _showMessage;

        public WpfLogger(Action<string> showMessage)
        {
            _showMessage = showMessage;
        }

        public void ShowError(string message)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message)
        {
            _showMessage(message);
        }
    }
}
