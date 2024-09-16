using CommonLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.NetWork
{
    public class IpVision
    {
        public static List<string> GetIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ips = new List<string>();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ips.Add(ip.ToString());
                }
            }
            return ips;
        }
       
        public static async Task BroadClient(string IpAddress, ILogger logger)
        {
            IpAddress = "235.5.5.11";
            using var udpClient = new UdpClient(8001);
            var brodcastAddress = IPAddress.Parse(IpAddress); // хост для отправки данных 
                                                                   // присоединяемся к группе
            udpClient.JoinMulticastGroup(brodcastAddress);
            logger.ShowMessage("Начало прослушивания сообщений");
            try
            {
                while (true)
                {
                    //await Task.Delay(1000);
                    var result = await udpClient.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(result.Buffer);
                    if (message == "END") break;
                    logger.ShowMessage(message);
                }
            }
            catch (Exception ex)
            {

                logger.ShowError(ex.Message);
            }
            
            await Task.Delay(1000);
            // отсоединяемся от группы
            udpClient.DropMulticastGroup(brodcastAddress);
            logger.ShowMessage("Udp-клиент завершил свою работу");
        }
        public static async Task BroadSender(string IpAddress, ILogger logger = null)
        {
            await Task.Delay(1000);
            IpAddress = "235.5.5.11";
            var messages = new string[] { "Hello World!", "Hello METANIT.COM", "Hello work", "END" };
            var brodcastAddress = IPAddress.Parse(IpAddress); ; // хост для отправки данных 
            using var udpSender = new UdpClient();
            Console.WriteLine("Начало отправки сообщений...");
            // отправляем сообщения
            foreach (var message in messages)
            {
                Console.WriteLine($"Отправляется сообщение: {message}");
                byte[] data = Encoding.UTF8.GetBytes(message);
                await udpSender.SendAsync(data, data.Length, new IPEndPoint(brodcastAddress, 8001));
                await Task.Delay(1000);
            }
        }
    }
}
