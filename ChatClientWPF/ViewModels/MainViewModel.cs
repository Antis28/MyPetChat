using ChatClientWPF.Handlers;
using ChatClientWPF.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.CodeGenerators;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using TcpServer.Handlers;
using TcpServer.Models;

namespace ChatClientWPF.ViewModels
{
    [GenerateViewModel]
    public partial class MainViewModel : ViewModelBase
    {
        [GenerateProperty]
        string userName;
        [GenerateProperty]
        public string ip;
        [GenerateProperty]
        public int port;
        [GenerateProperty]
        string status;

        [GenerateProperty]
        ObservableCollection<string> chat = new ObservableCollection<string>();

        [GenerateProperty]
        ObservableCollection<string> userNames = new ObservableCollection<string>();

        [GenerateProperty]
        string message;



        TcpClient _client;
        StreamReader _reader;
        StreamWriter _writer;
        ChatJsonConverter _chatJsonConverter = new ChatJsonConverter();
        CommandsHandler _commandsHandler = new CommandsHandler();

        [GenerateCommand]
        void Login() => Status = "User: " + userName;
        bool CanLogin() => !string.IsNullOrEmpty(userName);

        public MainViewModel()
        {
            ip = "192.168.1.105";
            port = 5050;
            userName = RandomeUserName();
            StartClient();
        }

        public AsyncCommand ConnectCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            _client = new TcpClient();
                            _client.Connect(Ip, Port);
                            PrintInUI($"Подключение к ip:{ip}:{port}");
                            _reader = new StreamReader(_client.GetStream());
                            _writer = new StreamWriter(_client.GetStream());
                            _writer.AutoFlush = true;

                            Logining();

                            PrintInUI($"Подключение успешно!");
                        }
                        catch (Exception ex)
                        {
                            PrintInUI($"Ошибка: {ex.Message}");
                        }
                    });
                    // Если не подключен к серверу
                }, () => _client == null || _client?.Connected == false);
            }
        }

        public AsyncCommand GetUsersCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            GetUsers();
                        }
                        catch (Exception ex)
                        {
                            PrintInUI($"Ошибка: {ex.Message}");
                        }
                    });
                    // Если не подключен к серверу
                }, () => _client != null || !(_client?.Connected == false));
            }
        }

        [GenerateCommand]
        void DisconnectCommand(object obj)
        {
            if (_client == null) return;

            SendCloseAsync();
        }


        public AsyncCommand OpenFileCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                    {

                        //FileDialogs.Save();
                        //FileDialogs.msg();
                        SendFileAsync(FileDialogs.open());
                    });
                });
            }
        }

        public AsyncCommand SendCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return SendMsgAsync($"{UserName}: {Message}");

                    //});
                }, () => _client?.Connected == true, !string.IsNullOrWhiteSpace(Message));


            }
        }

        private Task SendCloseAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var cmd = _chatJsonConverter.WriteToJson(new CommandMessage()
                    {
                        Command = _commandsHandler.CommandToString(TcpCommands.CloseConnection),
                        Argument = null
                    });

                    SendBigSizeTCP(cmd);
                    Message = string.Empty;

                }
                catch (Exception ex)
                {
                    PrintInUI($"Ошибка: {ex.Message}");
                }
            });
        }

        private Task SendMsgAsync(string msg)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {

                    var cmd = _chatJsonConverter.WriteToJson(new CommandMessage()
                    {
                        Command = "Message",
                        Argument = msg
                    });

                    SendBigSizeTCP(cmd);

                    PrintInUI(msg);
                    Message = string.Empty;

                }
                catch (Exception ex)
                {
                    PrintInUI($"Ошибка: {ex.Message}");
                }
            });
        }

        private Task SendFileAsync(string fileName)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {

                    var cmdJs = _chatJsonConverter.WriteToJson(new CommandMessage()
                    {
                        Command = _commandsHandler.CommandToString(TcpCommands.FileTransfer),
                        Argument = System.IO.Path.GetFileName(fileName)
                    });
                    PrintInUI($"Отправка файkа: {fileName}");
                    SendBigSizeTCP(cmdJs);
                    SendBigSizeFileTCP(fileName);

                    PrintInUI($"Файл отправлен: {fileName}");
                    Message = string.Empty;

                }
                catch (Exception ex)
                {
                    PrintInUI($"Ошибка: {ex.Message}");
                }
            });
        }

        private void StartClient()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        if ((_client?.Connected) == true)
                        {
                            //Прием данных от сервера                           
                            var line = ReceivingBigBufferTCP();
                            var cmd = _chatJsonConverter.ReadFromJson(line);

                            if (!string.IsNullOrEmpty(line))
                            {
                                HandleMessage(cmd);
                            }
                            else
                            {
                                _client.Close();
                                PrintInUI("Ошибка подключения!");
                            }
                        }
                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        PrintInUI($"Ошибка: {ex.Message}");
                    }
                }

            });
        }

        private void Logining()
        {
            var cmd = _chatJsonConverter.WriteToJson(new CommandMessage()
            {
                Command = "Login",
                Argument = userName
            });
            SendBigSizeTCP(cmd);

            GetUsers();
        }


        /// <summary>
        /// Обработка данных с сервера
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void HandleMessage(CommandMessage cmd)
        {
            var tcpCmd = _commandsHandler.RecognizeCommand(cmd.Command);

            switch (tcpCmd)
            {
                case TcpCommands.CloseConnection:
                    return;
                case TcpCommands.GetUsers:
                    VisualiseUserList(cmd);
                    break;
                case TcpCommands.Message:
                    PrintInUI(cmd.Argument);
                    break;
                case
                    TcpCommands.Login:
                    PrintInUI(cmd.Argument);
                    GetUsers();
                    break;
                default:
                    break;
            }
        }

        private string RandomeUserName()
        {
            var names = new string[] { "Biser", "Tiser", "Ruser", "Niser", "Miser", "Cuser", "User", "Diser" };
            var r = new Random((int)DateTime.Now.Ticks);
            return names[r.Next(names.Length)];
        }

        /// <summary>
        /// Запрос списка пользователей
        /// </summary>
        private void GetUsers()
        {
            var cmd = _chatJsonConverter.WriteToJson(new CommandMessage()
            {
                Command = _commandsHandler.CommandToString(TcpCommands.GetUsers),
                Argument = null
            });

            SendBigSizeTCP(cmd);

            Message = string.Empty;
        }

        private void SendBigSize(string text)
        {
            var socket = _client.Client;
            byte[] data = Encoding.Default.GetBytes(text);
            socket.Send(BitConverter.GetBytes(data.Length), 0, 4, 0);
            socket.Send(data);
        }

        #region LowLevel transfer
        private void SendFileInByte(string fileName)
        {
            var stream = File.Open(fileName, FileMode.Open);
            //stream.Read



            //var socket = _client.Client;
            //byte[] data = Encoding.Default.GetBytes(text);
            //socket.Send(BitConverter.GetBytes(data.Length), 0, 4, 0);
            //socket.Send(data);
        }
        /// <summary>
        /// Отправка данных серверу
        /// </summary>
        /// <param name="cmdJs"></param>
        private void SendBigSizeTCP(string cmdJs)
        {
            // сообщение для отправки
            var message = cmdJs; // "Hello METANIT.COM";
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
            // Console.WriteLine("Сообщение отправлено");
        }

        /// <summary>
        /// Отправка данных
        /// </summary>
        /// <param name="message">сообщение для отправки</param>
        private void SendBigSizeFileTCP(string fileName)
        {
            // получаем NetworkStream для взаимодействия с принимающей стороной
            var stream = _client.GetStream();
            using (FileStream fileStream = File.OpenRead(fileName))
            {
               // byte[] buffer = new byte[4096];
                //int bytesRead;
                var length = fileStream.Length;
                byte[] size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));
                stream.Write(size,0, size.Length);
                // отправляем данные
                fileStream.CopyTo(stream);
                //stream.Write(data, 0, data.Length);
            }


            //var f1 = size1;
            //var f2 = data.Length;
            //// определяем размер данных
            //byte[] size = BitConverter.GetBytes(data.Length);
            // отправляем размер данных
            //stream.Write(size, 0, 4);
            // отправляем данные
           // stream.Write(data, 0, data.Length);
        }

        private void SendFile(string fileName)
        {
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                NetworkStream stream = _client.GetStream();
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }
                stream.Flush();
                fileStream.Close();
                //stream.Close();
            }
        }


        /// <summary>
        /// Прием данных от сервера
        /// </summary>
        /// <returns></returns>
        private string ReceivingBigBufferTCP()
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
            int bytes = stream.Read(data, 0, size);

            var message = Encoding.UTF8.GetString(data, 0, bytes);
            return message;
        }
        #endregion

        #region Visualise
        private void VisualiseUserList(CommandMessage cmd)
        {
            var co = _chatJsonConverter.ReadFromJson<List<ClientObject>>(cmd.Argument);
            RunInUi(() =>
            {
                userNames.Clear();
                //foreach (var item in messeges)
                //{
                //    lbChat.Items.Add(item);
                //}
                //if (lbChat.Items.Count > 0)
                //{
                //    lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
                //}
                foreach (var item in co)
                {
                    userNames.Add(item.UserName);
                }
            });
        }

        private void PrintInUI(string message)
        {
            RunInUi(() => Chat.Add(message));
        }

        private void RunInUi(Action action)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                action();
            });

        }
        #endregion
    }
}
