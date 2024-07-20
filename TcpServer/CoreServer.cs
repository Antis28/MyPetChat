using TcpServer.ViewModels;
using CommonLibrary.Interfaces;


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