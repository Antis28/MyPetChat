using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    public class CoreServer
    {
        static TcpListener _listener = new TcpListener(System.Net.IPAddress.Any, 5050);
        static List<ConnectedClient> _clients = new List<ConnectedClient>();
        static ILogger _loger;

        public static void StartServer(ILogger loger)
        {
            _listener.Start();
            _loger = loger;
            _loger.ShowMessage("Сервер запущен!");
            var client = _listener.AcceptTcpClient();
            Task.Factory.StartNew(() =>
            {
                var streamReader = new StreamReader(client.GetStream());

                ConnectedClient connectedClient = Loginning(client, streamReader);
                ClientHandler(connectedClient, streamReader);
            });
        }

        private static void ClientHandler(ConnectedClient connectedClient, StreamReader streamReader)
        {
            var client = connectedClient.Client;
            while (client.Connected)
            {
                try
                {
                    streamReader = new StreamReader(client.GetStream());
                    var line = streamReader.ReadLine();
                    _loger.ShowMessage($"{line}");
                    SendToAllClients(line);
                }
                catch (Exception ex)
                {
                    _loger.ShowError(ex.Message);
                }

            }            
        }

        private static ConnectedClient Loginning(TcpClient client, StreamReader streamReader)
        {
            while (client.Connected)
            {
                var line = streamReader.ReadLine();

                var searchString = "Login: ";
                var nick = line.Replace(searchString, "");
                if (line.Contains(searchString) && !string.IsNullOrWhiteSpace(nick))
                {
                    if (_clients.FirstOrDefault(s => s.Name == nick) == null)
                    {
                        var connectedClient = new ConnectedClient(client, nick);
                        _clients.Add(connectedClient);
                        _loger.ShowMessage($"Подключен пользователь {nick}.");

                        return connectedClient;
                    }
                }
                else
                {
                    var streamWriter = new StreamWriter(client.GetStream());
                    streamWriter.AutoFlush = true;
                    _loger.ShowMessage($"Пользователь {nick} уже в чате.");
                    client.Client.Disconnect(false);
                }
            }
            throw new Exception("Not Loginning");
        }

        private static async void SendToAllClients(string message)
        {
            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < _clients.Count; i++)
                {
                    try
                    {
                        if (_clients[i].Client.Connected)
                        {
                            var streamWriter = new StreamWriter(_clients[i].Client.GetStream());
                            streamWriter.AutoFlush = true;
                            streamWriter.WriteLine(message);
                        }
                        else
                        {
                            _clients.RemoveAt(i);
                            _loger.ShowMessage($"Пользователь {_clients[i].Name} вышел из чата.");
                        }


                    }
                    catch (Exception ex)
                    {
                        _loger.ShowError(ex.Message);
                    }
                }
            });
        }
    }
}
