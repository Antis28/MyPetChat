using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Handlers;
using TcpServer.ViewModels;

namespace TcpServer
{
    public class CoreServer
    {
        public static void StartServer(ILogger loger)
        {
            ServerObject server = new ServerObject(loger);// создаем сервер
            server.ListenAsync(); // запускаем сервер            
        }

    }
}