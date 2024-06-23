using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
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
        static void Main(string[] args)
        {
            Task.Factory.StartNew(async () =>
            {
                await CoreServer.StartServer(new Logger());
            });
            Console.ReadLine();

            //using (var host = new ServiceHost(typeof(WcfChat.ServiceChat)))
            //{
            //    host.Open();

            //    Console.WriteLine("Host started!");
            //    Console.ReadLine();
            //}
        }
    }
}
