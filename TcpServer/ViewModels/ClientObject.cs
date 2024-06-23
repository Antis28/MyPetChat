using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Handlers;
using TcpServer.Models;

namespace TcpServer.ViewModels
{
    internal class ClientObject
    {
        protected internal string Id { get; } = Guid.NewGuid().ToString();
        protected string UserName { get; set; }
        protected internal StreamWriter Writer { get; }
        protected internal StreamReader Reader { get; }

        ChatJsonConverter chatJsonConverter = new ChatJsonConverter();
        CommandsHandler commandsHandler = new CommandsHandler();
        ILogger _logger;

        TcpClient _client;
        ServerObject _server; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject, ILogger logger)
        {
            _client = tcpClient;
            _server = serverObject;
            _logger = logger;
            // получаем NetworkStream для взаимодействия с сервером
            var stream = _client.GetStream();
            // создаем StreamReader для чтения данных
            Reader = new StreamReader(stream);
            // создаем StreamWriter для отправки данных
            Writer = new StreamWriter(stream);
        }

        public async Task ProcessAsync()
        {
            try
            {
                await Loginning();
                ClientHandler();
            }
            catch (Exception e)
            {
                var message = $"{UserName} покинул чат!\n{e.Message}";
                _logger.ShowError(message);
                await _server.BroadcastMessageAsync(message, Id);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                _server.RemoveConnection(Id);
            }
        }
        // закрытие подключения
        protected internal void Close()
        {
            Writer.Close();
            Reader.Close();
            _client.Close();
        }


        private async Task Loginning()
        {
            // получаем имя пользователя  
            var line = await Reader.ReadLineAsync();
            var cmd = chatJsonConverter.ReadFromJson(line);
            
            var comandType = commandsHandler.RecognizeCommand(cmd.Command);
            if (TcpCommands.Login == comandType)
            {
                await HandleLogin(cmd.Argument);
            }
            else
            {
                throw new Exception("Not Loginning");
            }
        }

        private async void ClientHandler()
        {
            // в бесконечном цикле получаем сообщения от клиента           
            try
            {
                while (_client.Connected)
                {
                    var message = HandlerBigBuffer();

                    var connected = await isClosed(message);
                    if (!connected) break;

                    _logger.ShowMessage($"{message}");
                    await _server.BroadcastMessageAsync(message, Id);
                }
            }
            catch (Exception ex)
            {
                _logger.ShowError(ex.Message);
                Close();
            }
        }

        /// <summary>
        /// Server Buff handler
        /// </summary>
        /// <param name="connectedClient"></param>
        /// <returns></returns>
        private string HandlerBigBuffer()
        {
            var cl = _client.Client;
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
        private async Task<bool> isClosed(string line)
        {
            //if (line.Contains(searchString))
            //{
            //    var message = $"{UserName} вышел из чата!";

            //    _logger.ShowMessage(message);
            //    // посылаем сообщение в чат всем подключенным пользователям
            //    await _server.BroadcastMessageAsync(message, Id);
            //    Close();
            //    return false;
            //}
            return true;
        }

        //private async Task<bool> Handle(CommandMessage cmd)
        //{
        //    //var comandType = commandsHandler.RecognizeCommand(cmd.Command);
        //    //switch (comandType)
        //    //{
        //    //    case TcpCommands.Login:

        //    //        break;
        //    //    case TcpCommands.CloseConnection:
        //    //       break;
        //    //};
            

        //}

        private async Task HandleLogin(string UserName)
        {
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                
                var message = $"{UserName} вошел в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                _logger.ShowMessage(message);
                await _server.BroadcastMessageAsync(message, Id);
            }
            else
            {
                var message = $"Пустое имя пользователя";
                _logger.ShowError(message);
                throw new Exception(message);
            }
        }
    }
}
