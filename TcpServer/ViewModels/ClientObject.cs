using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpServer.Handlers;
using TcpServer.Models;

namespace TcpServer.ViewModels
{
    internal class ClientObject
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string UserName { get; set; }
        protected internal StreamWriter Writer { get; }
        protected internal StreamReader Reader { get; }

        ChatJsonConverter _chatJsonConverter = new ChatJsonConverter();
        CommandsHandler _commandsHandler = new CommandsHandler();
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

        public void Process()
        {
            _logger.ShowMessage($"Process Thread: {Thread.CurrentThread.ManagedThreadId}");
            try
            {
                var connected = _client.Connected;
                while (_client.Connected && connected)
                {
                    var message = ReceivingBigBufferTCP();

                    connected = HandleMessage(message);
                }
            }
            catch (Exception e)
            {
                var message = $"{UserName} покинул чат!\n{e.Message}";
                _logger.ShowError(message);
                _server.BroadcastMessage(message, Id);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                _server.RemoveConnection(Id);
                var message = $"{UserName} покинул чат!";
                _logger.ShowMessage(message);
                Close();
            }
        }
        // закрытие подключения
        protected internal void Close()
        {
            Writer.Close();
            Reader.Close();
            _client.Close();
        }



        private void Loginning(CommandMessage cmd)
        {
            UserName = cmd.Argument;
            if (!string.IsNullOrWhiteSpace(UserName))
            {

                var message = $"{UserName} вошел в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                _logger.ShowMessage(message);
                var cmdMessage = _chatJsonConverter.WriteToJson(new()
                {
                    Command = _commandsHandler.CommandToString(TcpCommands.Login),
                    Argument = message,
                });
                _server.BroadcastMessage(cmdMessage, Id);
            }
            else
            {
                var message = $"Пустое имя пользователя";
                _logger.ShowError(message);
                throw new Exception(message);
            }
        }



        /// <summary>
        /// Server Buff handler
        /// Прием данных от клиента
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
        /// <summary>
        /// Прием строковых данных от клиента
        /// </summary>
        /// <returns></returns>
        private string ReceivingBigBufferTCP()
        {
            var (data, byteLength) = ReceivingBigBufferRawDataTCP();

            var message = Encoding.UTF8.GetString(data, 0, byteLength);
            return message;
        }
        /// <summary>
        /// Прием сырых данных от клиента
        /// </summary>
        /// <returns></returns>
        private Tuple<byte[], int> ReceivingBigBufferRawDataTCP()
        {
            // получаем объект NetworkStream для взаимодействия с клиентом
            var stream = _client.GetStream();
            // буфер для считывания размера данных
            byte[] sizeBuffer = new byte[4];
            // сначала считываем размер данных
            var i = stream.Read(sizeBuffer, 0, sizeBuffer.Length);
            // узнаем размер и создаем соответствующий буфер
            int size = BitConverter.ToInt32(sizeBuffer, 0);
            // создаем соответствующий буфер
            byte[] data = new byte[size];
            // считываем собственно данные
            int byteLength = stream.Read(data, 0, size);

            return new Tuple<byte[], int>(data, byteLength);
        }


        /// <summary>
        /// Отправка данных клиенту
        /// </summary>
        /// <param name="message">сообщение для отправки</param>
        public void SendBigSizeTCP(string message)
        {
            // получаем NetworkStream для взаимодействия с сервером
            var stream = _client.GetStream();
            // считыванием строку в массив байт
            byte[] data = Encoding.UTF8.GetBytes(message);
            // определяем размер данных
            byte[] size = BitConverter.GetBytes(data.Length);
            // отправляем размер данных
            stream.Write(size, 0, 4);
            // отправляем данные
            stream.Write(data, 0, data.Length);
        }
        /// <summary>
        /// Отправка данных клиенту
        /// </summary>
        /// <param name="message">сообщение для отправки</param>
        public void SendBigSizeTCP(byte[] data, int size1)
        {
            // получаем NetworkStream для взаимодействия с сервером
            var stream = _client.GetStream();
            // считыванием строку в массив байт
            //byte[] data = Encoding.UTF8.GetBytes(message);
            //// определяем размер данных
            var f1 = size1;
            var f2 = data.Length;
            byte[] size = BitConverter.GetBytes(data.Length);
            // отправляем размер данных
            stream.Write(size, 0, 4);
            // отправляем данные
            stream.Write(data, 0, data.Length);
        }

        private bool HandleMessage(string line)
        {
            var cmd = _chatJsonConverter.ReadFromJson(line);
            var cm = _commandsHandler.RecognizeCommand(cmd.Command);
            //_logger.ShowMessage($"HandleMessage: {cmd.Command}-{cmd.Argument}");
            switch (cm)
            {
                case TcpCommands.CloseConnection:
                    return false;
                case TcpCommands.Login:
                    Loginning(cmd);
                    break;
                case TcpCommands.GetUsers:
                    SendUserList();
                    break;
                case TcpCommands.Message:
                    _server.BroadcastMessage(line, Id);
                    break;
                case TcpCommands.FileTransfer:
                    SendFile(cmd);
                    break;
                default:
                    break;
            }
            return true;
        }

        private void SendUserList()
        {
            var rtrtr = JsonConvert.SerializeObject(_server.Clients, Formatting.None);
            var getUsersCommand = new CommandMessage()
            {
                Command = _commandsHandler.CommandToString(TcpCommands.GetUsers),
                Argument = JsonConvert.SerializeObject(_server.Clients, Formatting.None)
            };
            var message = _chatJsonConverter.WriteToJson(getUsersCommand);

            SendBigSizeTCP(message);
        }
        private void SendFile(CommandMessage cmd)
        {
            var message = _chatJsonConverter.WriteToJson(cmd);
            //var (bytes, size) = ReceivingBigBufferRawDataTCP();
            ReceiveFile1(cmd.Argument);
        }

        /// <summary>
        /// Принять файл
        /// </summary>
        /// <param name="savePath"></param>
        public void ReceiveFile(string savePath)
        {
            NetworkStream stream = _client.GetStream();
            FileStream fileStream = File.Create(savePath);
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
            }
            _logger.ShowMessage(savePath);
            fileStream.Close();
        }

        /// <summary>
        /// Принять файл
        /// </summary>
        /// <param name="savePath"></param>
        public void ReceiveFile1(string savePath)
        {
            var stream = _client.GetStream();

            byte[] buf = new byte[65536];
            ReadBytes(sizeof(long), buf);
            long remainingLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buf, 0));

            using var file = File.Create(savePath);
            while (remainingLength > 0)
            {
                int lengthToRead = (int)Math.Min(remainingLength, buf.Length);
                ReadBytes(lengthToRead, buf);
                file.Write(buf, 0, lengthToRead);
                remainingLength -= lengthToRead;
            }
        }
        void ReadBytes(int howmuch, byte[] buf)
        {
            var stream = _client.GetStream();
            int readPos = 0;
            while (readPos < howmuch)
            {
                var actuallyRead = stream.Read(buf, readPos, howmuch - readPos);
                if (actuallyRead == 0)
                    throw new EndOfStreamException();
                readPos += actuallyRead;
            }
        }

    }
}
