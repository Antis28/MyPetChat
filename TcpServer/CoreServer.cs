using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Handlers;

namespace TcpServer
{
    public class CoreServer
    {
        static TcpListener _listener = new TcpListener(System.Net.IPAddress.Any, 5050);
        static List<ConnectedClient> _clients = new List<ConnectedClient>();
        static ILogger _loger;

        static private string _closecmd = "Close connection";
        ChatJsonConverter chatJsonConverter = new ChatJsonConverter();


        public static void StartServer(ILogger loger)
        {
            try
            {
                _listener.Start();
                _loger = loger;
                _loger.ShowMessage("Сервер запущен!");
                while (true)
                {
                    var client = _listener.AcceptTcpClient();
                    Task.Factory.StartNew(() =>
                    {
                        var streamReader = new StreamReader(client.GetStream());

                        ConnectedClient connectedClient = Loginning(client, streamReader);
                        ClientHandler(connectedClient);
                    });
                }
            }
            catch (Exception e)
            {
                _loger.ShowError(e.Message);
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

        private static async void ClientHandler(ConnectedClient connectedClient)
        {
            var client = connectedClient.Client;
            try
            {
                while (client.Connected)
                {
                    StreamReader streamReader = new StreamReader(connectedClient.Client.GetStream());
                    var line = streamReader.ReadLine();

                    var connected = await isClosed(line, connectedClient);
                    if (!connected) break;

                    _loger.ShowMessage($"{line}");
                    await SendToAllClientsAsync(connectedClient, line);
                }
            }
            catch (Exception ex)
            {
                _loger.ShowError(ex.Message);
                client = null;
            }
        }

        private static async Task<bool> isClosed(string line, ConnectedClient connectedClient)
        {
            

            // Команда закрытия соединения
            var searchString = _closecmd;
            if (line.Contains(searchString))
            {
                _loger.ShowMessage($"{connectedClient.Name} вышел из чата!");
                await SendToAllClientsAsync(connectedClient, $"{connectedClient.Name} вышел из чата!");
                connectedClient.Client.Close();
                return false;
            }
            //var searchString2 = "List users";
            //if (line.Contains(searchString2))
            //{

            //    _loger.ShowMessage($"{connectedClient.Name} вышел из чата!");
            //    await SendToAllClientsAsync(connectedClient, $"{connectedClient.Name} вышел из чата!");
            //    connectedClient.Client.Close();
            //    return false;
            //}           
            return true;
        }

        private static Task SendToAllClientsAsync(ConnectedClient connectedClient, string message)
        {
            return Task.Factory.StartNew(() =>
            {
                SendToAllClients(connectedClient, message);
            });
        }

        private static void SendToAllClients(ConnectedClient connectedClient, string message)
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                if (_clients[i].Name == connectedClient.Name) continue;
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
        }
    }
}
