using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.ViewModels
{
    internal class ServerObject
    {
        TcpListener _tcpListener = new TcpListener(System.Net.IPAddress.Any, 5050); // сервер для прослушивания
        List<ClientObject> _clients = new List<ClientObject>(); // все подключения
        ILogger _loger;

        public ServerObject(ILogger logger)
        {
            _loger = logger;
        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            var client = _clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null) _clients.Remove(client);
            client?.Close();
        }
        // прослушивание входящих подключений
        protected internal async Task ListenAsync()
        {
            try
            {
                _tcpListener.Start();
                _loger.ShowMessage("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();

                    ClientObject clientObject = new ClientObject(tcpClient, this, _loger);
                    _clients.Add(clientObject);

                    await Task.Factory.StartNew(clientObject.ProcessAsync, TaskCreationOptions.LongRunning);
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
        protected internal async Task BroadcastMessageAsync(string message, string id)
        {
            foreach (var client in _clients)
            {
                if (client.Id != id) // если id клиента не равно id отправителя
                {
                    await client.Writer.WriteLineAsync(message); //передача данных
                    await client.Writer.FlushAsync();
                }
            }
        }
        // отключение всех клиентов
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
