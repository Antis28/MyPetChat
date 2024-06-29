using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Models;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

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
        private const string loginSuccess = "Login success";
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
                case TcpCommands.LoginSuccess:
                    return _chatCommands.LoginSuccess;
                default:
                    break;
            }
            throw new Exception("Команда не распознана");
        }

        public void ToJsonFileAsync()
        {
            Task.Factory.StartNew(async () =>
            {
                ChatCommands cc = new ChatCommands()
                {
                    CloseConection = closeCommand,
                    Login = loginCommand,
                    GetUsers = getUsersCommand,
                    Message = messageCommand,
                    FileTransfer = fileTransferCommand,
                    UpdateUserName = updateUserName,
                    LoginSuccess = loginSuccess,
                };
                string serializedMsg = JsonConvert.SerializeObject(cc, Formatting.Indented);
                using (var file = new StreamWriter("Commands.json", false, Encoding.UTF8))
                {
                    await file.WriteAsync(serializedMsg);
                }
            });
        }
        public void FromJsonFile()
        {
            using (var file = new StreamReader("Commands.json", Encoding.UTF8))
            {
                var t = file.ReadToEnd();
                _chatCommands = JsonConvert.DeserializeObject<ChatCommands>(t);
            }
        }
    }
}
