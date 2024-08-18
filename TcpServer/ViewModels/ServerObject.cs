using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using CommonLibrary.Interfaces;
using CommonLibrary.NetWork;

namespace TcpServer.ViewModels
{
    internal class ServerObject
    {
        TcpListener _tcpListener = new TcpListener(System.Net.IPAddress.Any, 5050); // сервер для прослушивания
        List<ClientObject> _clients = new(); // все подключения
        ILogger _logger;

        internal List<ClientObject> Clients { get => _clients;}

        public ServerObject(ILogger logger)
        {
            _logger = logger;
        }
       
        // прослушивание входящих подключений
        protected internal void ListenAsync()
        {
            try
            {
                _tcpListener.Start();
                _logger.ShowMessage("Адреса для подключения:");
                var ips = IpVision.GetIp();
                foreach (var item in ips)
                {
                    _logger.ShowMessage(item);
                }
                _logger.ShowMessage("Сервер запущен. Ожидание подключений...");

                Task.Run(async () => { await IpVision.BroadSender(""); });
               


                while (true)
                {
                    TcpClient tcpClient = _tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this, _logger);
                    _clients.Add(clientObject);

                    _logger.ShowMessage($"ListenAsync Thread: {Thread.CurrentThread.ManagedThreadId}");
                    Task.Run(clientObject.Process);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        // трансляция сообщения подключенным клиентам
        protected internal void BroadcastMessage(string message, string id)
        {
            foreach (var client in _clients)
            {
                if (client.Id != id) // если id клиента не равно id отправителя
                {
                    client.Send(message);
                }
            }
        }

        // отключение соеденения клиента
        protected internal void RemoveConnectionFromList(string id)
        {
            // получаем по id закрытого подключение
            var client = _clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null) _clients.Remove(client);
            client?.Close();
        }
        // отключение всех клиентов, остановка сервера
        protected internal void Disconnect()
        {
            foreach (var client in _clients)
            {
                client.Close(); //отключение клиента
            }
            _tcpListener.Stop(); //остановка сервера
        }
    }
}
