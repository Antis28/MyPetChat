using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Models;

namespace TcpServer.Handlers
{
    internal class CommandsHandler
    {
        private const string closeCommand = "Close connection";
        private const string loginCommand = "Login";
        private const string getUsersCommand = "Get users";
        private const string messageCommand = "Message";


        /// <summary>
        /// Распознать команду
        /// </summary>
        /// <param name="searchCommand"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TcpCommands RecognizeCommand(string searchCommand)
        {
            switch (searchCommand)
            {
                case closeCommand:
                    return TcpCommands.CloseConnection;
                case loginCommand:
                    return TcpCommands.Login;
                case getUsersCommand:
                    return TcpCommands.GetUsers;
                case messageCommand:
                    return TcpCommands.Message;
                default:
                    break;
            }
            throw new Exception("Команда не распознана");
        }
        public string CommandToString(TcpCommands searchCommand)
        {
            switch (searchCommand)
            {
                case TcpCommands.CloseConnection:
                    return closeCommand;
                case TcpCommands.Login:
                    return loginCommand;
                case TcpCommands.GetUsers:
                    return getUsersCommand;
                case TcpCommands.Message:
                    return messageCommand;
                default:
                    break;
            }
            throw new Exception("Команда не распознана");
        } 
    }
}
