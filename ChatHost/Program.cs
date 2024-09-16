using System;
using System.Threading.Tasks;
using TcpServer;

// Вот та самая строка теперь здесь
// указывает, что log4net будет брать настройки из .config файла
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
//Эта директива приведет к тому, что log4net будет искать конфигурационный файл
//с именем ConsoleApp.exe.config в базовом
//каталоге приложения (т.е. в каталоге, содержащем ConsoleApp.exe )


namespace ChatHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task.Factory.StartNew(() =>
            {
                CoreServer.StartServer(new Logger());
            });
            Console.ReadLine();
        }
    }
}
