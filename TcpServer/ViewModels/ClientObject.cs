using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpServer.Handlers;
using TcpServer.Models;
using TcpServer.ViewModels.ClientHandlers;

namespace TcpServer.ViewModels
{
    internal class ClientObject
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string UserName { get; set; }        

        ChatJsonConverter _chatJsonConverter = new ChatJsonConverter();
        CommandConverter _commandsHandler = new CommandConverter();
        ILogger _logger;

        TcpClient _client;
        ServerObject _server; // объект сервера

        FileHandler _fileHandler;
        DataTransfeHandler _dataTransferHandler;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject, ILogger logger)
        {
            _client = tcpClient;
            _server = serverObject;
            _logger = logger;
            _fileHandler = new(_client, _logger);
            _dataTransferHandler = new(_client, _logger);        
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
                var message = $"вошел в чат!";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                _logger.ShowMessage($"{UserName}: {message}");
               
                var cmdMessage = NewCommand(TcpCommands.Login, message);                
                _server.BroadcastMessage(cmdMessage, Id);
                cmdMessage = NewCommand(TcpCommands.LoginSuccess, "Подключение успешно!");
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
                    _fileHandler.ReceiveFile1(cmd);
                    break;
                case TcpCommands.UpdateUserName:
                    if (UserName == cmd.UserName) break;

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

        private string NewCommand(TcpCommands commandName, string argument = null)
        {
            return _chatJsonConverter.WriteToJson(new CommandMessage()
            {
                Command = _commandsHandler.CommandToString(commandName),
                UserName = UserName,
                Argument = argument,
                UserID = Id,
            });
        }
    }
}
