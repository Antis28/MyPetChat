using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Models;

namespace TcpServer.Handlers
{
    internal class CommandsHandler
    {
        private string _closecmd = "Close connection";

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
                case "Close connection":
                    return TcpCommands.CloseConnection;
                case "Login":
                    return TcpCommands.Login;
                default:
                    break;
            }
            throw new Exception("Команда не распознана");
        }
       
    }
}
