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
        static TcpListener _listener = new TcpListener(System.Net.IPAddress.Any, 5050);
        static List<ConnectedClient> _clients = new List<ConnectedClient>();
        static ILogger _loger;

        static private string _closecmd = "Close connection";
        static ChatJsonConverter chatJsonConverter = new ChatJsonConverter();


        public static async Task StartServer(ILogger loger)
        {
            _loger = loger;
            ServerObject server = new ServerObject(loger);// создаем сервер
            await server.ListenAsync(); // запускаем сервер
        }

        private static ConnectedClient Loginning(TcpClient client, StreamReader streamReader)
        {
            while (client.Connected)
            {
                var line = streamReader.ReadLine();
                var cmd = chatJsonConverter.ReadFromJson(line);
                var nick = cmd.Argument;

                if (cmd.Command == "Login" && !string.IsNullOrWhiteSpace(nick))
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
                    //var line = streamReader.ReadLine();
                    //Encoding.UTF8.GetString(

                    //var t1 = await streamReader.ReadToEndAsync();
                    
                    var line = HandlerBigBuffer(connectedClient);
                    // var cmd = chatJsonConverter.ReadFromJson(t);



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


        /// <summary>
        /// Server Buff handler
        /// </summary>
        /// <param name="connectedClient"></param>
        /// <returns></returns>
        private static string HandlerBigBuffer(ConnectedClient connectedClient)
        {
            var cl = connectedClient.Client.Client;
            byte[] sizeBuf = new byte[4];

            // Получаем размер первой порции
            cl.Receive(sizeBuf, 0, sizeBuf.Length, 0);
            // Преобразуем в целое число
            int size = BitConverter.ToInt32(sizeBuf, 0);
            MemoryStream memoryStream = new MemoryStream();

            while (size > 0)
            {
                byte[] buffer;
                // Проверяем чтобы буфер был не меньше необходимого для принятия
                if (size < cl.ReceiveBufferSize) buffer = new byte[size];
                else buffer = new byte[cl.ReceiveBufferSize];

                // Получим данные в буфер
                int rec = cl.Receive(buffer, 0, buffer.Length, 0);
                // Вычитаем размер принятых данных из общего размера
                size -= rec;
                // записываем в поток памяти
                memoryStream.Write(buffer, 0, buffer.Length);
            }
            memoryStream.Close();
            byte[] data = memoryStream.ToArray();
            memoryStream.Dispose();

            return Encoding.Default.GetString(data);
        }
    }
}
