using CommonLibrary;
using CommonLibrary.Interfaces;
using System;
using System.Collections.ObjectModel;

namespace ChatClientWPF.ViewModels
{
    internal class WpfLogger : ILogger
    {
        private ObservableCollection<CommandMessage> _chat;
        private Action<string> _showMessage;

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
