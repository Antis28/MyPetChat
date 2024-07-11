using ChatClientWPF.Handlers;
using ChatClientWPF.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Models;

namespace TcpServer.Handlers
{
    internal class CommandConverter
    {
        #region commands        
        private const string closeCommand = "Close connection";
        private const string loginCommand = "Login";
        private const string getUsersCommand = "Get users";
        private const string messageCommand = "Message";
        private const string fileTransferCommand = "File Transfer";
        private const string updateUserName = "Update user name";
        #endregion

        private ChatCommands _chatCommands;
        public CommandConverter()
        {
            FromJsonFile();
        }


        /// <summary>
        /// Распознать команду
        /// </summary>
        /// <param name="searchCommand"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TcpCommands RecognizeCommand(string searchCommand)
        {
            if (searchCommand == _chatCommands.CloseConection)
                return TcpCommands.CloseConnection;
            if (searchCommand == _chatCommands.Login)
                return TcpCommands.Login;
            if (searchCommand == _chatCommands.GetUsers)
                return TcpCommands.GetUsers;
            if (searchCommand == _chatCommands.Message)
                return TcpCommands.Message;
            if (searchCommand == _chatCommands.FileTransfer)
                return TcpCommands.FileTransfer;
            if (searchCommand == _chatCommands.UpdateUserName)
                return TcpCommands.UpdateUserName;
            if (searchCommand == _chatCommands.LoginSuccess)
                return TcpCommands.LoginSuccess;

            throw new Exception("Команда не распознана");
        }
        public string CommandToString(TcpCommands searchCommand)
        {
            switch (searchCommand)
            {
                case TcpCommands.CloseConnection:
                    return _chatCommands.CloseConection;
                case TcpCommands.Login:
                    return _chatCommands.Login;
                case TcpCommands.GetUsers:
                    return _chatCommands.GetUsers;
                case TcpCommands.Message:
                    return _chatCommands.Message;
                case TcpCommands.FileTransfer:
                    return _chatCommands.FileTransfer;
                case TcpCommands.UpdateUserName:
                    return _chatCommands.UpdateUserName;
                default:
                    break;
            }
            throw new Exception("Команда не распознана");
        }


        public void FromJsonFile()
        {
            using (var file = new StreamReader("Commands.json", Encoding.UTF8))
            {
                var t = file.ReadToEnd();
                _chatCommands = JSaver.Load<ChatCommands>(t);
            }
        }
    }
}
