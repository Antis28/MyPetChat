using CommonLibrary;
using CommonLibrary.Interfaces;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Threading;
using TcpServer.ViewModels.ClientHandlers;

namespace TcpServer.ViewModels
{
    internal class ClientObject
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string UserName { get; set; }

        public string IPAddress { get; set; }

        private readonly ChatJsonConverter _chatJsonConverter = new ChatJsonConverter();
        private readonly CommandConverter _commandsHandler = new CommandConverter();
        private readonly ILogger _logger;
        private readonly TcpClient _client;
        private readonly ServerObject _server; // объект сервера

        private FileAcceptanceProcessing _fileHandler;
        private DataTransfeHandler _dataTransferHandler;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject, ILogger logger)
        {
            _client = tcpClient;
            _server = serverObject;
            _logger = logger;

            _fileHandler = new(_client, _logger);
            _fileHandler.OnProgress += (message, percent) => _logger.ShowMessage(message);
            _fileHandler.OnComplete += (m, path) => _logger.ShowMessage($"Копирование завершено.\n{path}");

            _dataTransferHandler = new(_client);
        }

        public void Process()
        {
            _logger.ShowMessage($"Process Thread: {Thread.CurrentThread.ManagedThreadId}");
            try
            {
                var connected = _client.Connected;
                while (_client.Connected && connected)
                {
                    var message = _dataTransferHandler.ReceivingBigBufferTCP();
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
                _server.RemoveConnectionFromList(Id);
                var message = $"{UserName} покинул чат!";
                _logger.ShowMessage(message);
                Close();
            }
        }

        private void Loginning(CommandMessage cmd)
        {
            UserName = cmd.UserName;
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                IPAddress = cmd.IPAddress;
                var message = $"вошел в чат!";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                _logger.ShowMessage($"{UserName}: {message}");

                var cmdMessage = NewCommand(TcpCommands.Login, message, cmd.IPAddress);
                _server.BroadcastMessage(cmdMessage, Id);
                cmdMessage = NewCommand(TcpCommands.LoginSuccess, "Подключение успешно!", cmd.IPAddress);
                Send(cmdMessage);
            }
            else
            {
                var message = $"Пустое имя пользователя";
                _logger.ShowError(message);
                throw new Exception(message);
            }
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
                    // Принять файл от клиента
                    string savePath = cmd.Argument;

                    _fileHandler.ReceiveFile(savePath);
                    break;
                case TcpCommands.UpdateUserName:
                    if (UserName == cmd.UserName)
                    {
                        break;
                    }

                    UserName = cmd.UserName;
                    BroadcastUserList();
                    break;
                default:
                    break;
            }
            return true;
        }


        /// <summary>
        /// Отправить список пользователей
        /// </summary>
        private void SendUserList()
        {
            var argument = JsonConvert.SerializeObject(_server.Clients, Formatting.None);
            var message = NewCommand(TcpCommands.GetUsers, argument);
            Send(message);
        }
        private void BroadcastUserList()
        {
            var argument = JsonConvert.SerializeObject(_server.Clients, Formatting.None);
            var message = NewCommand(TcpCommands.GetUsers, argument);
            _server.BroadcastMessage(message, Id);
        }

        /// <summary>
        /// Отпавить сообщение
        /// </summary>
        /// <param name="message"></param>
        internal void Send(string message)
        {
            _dataTransferHandler.SendBigSizeTCP(message);
        }

        // закрытие подключения
        protected internal void Close()
        {
            _client.Close();
        }

        private string NewCommand(TcpCommands commandName, string argument = null, string ipAddress = null)
        {
            return _chatJsonConverter.WriteToJson(new CommandMessage
            {
                Command = _commandsHandler.CommandToString(commandName),
                UserName = UserName,
                Argument = argument,
                UserID = Id,
                IPAddress = ipAddress,
            });
        }
    }
}
