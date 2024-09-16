using CommonLibrary.Interfaces;
using TcpServer.ViewModels;


namespace TcpServer
{
    public class CoreServer
    {
        public static void StartServer(ILogger loger)
        {
            //new CommandConverter().ToJsonFileAsync();
            ServerObject server = new ServerObject(loger);// создаем сервер
            server.ListenAsync(); // запускаем сервер            
        }

    }
}