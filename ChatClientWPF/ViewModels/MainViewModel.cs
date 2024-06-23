using ChatClientWPF.Handlers;
using DevExpress.Mvvm;
using DevExpress.Mvvm.CodeGenerators;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Markup;
using TcpServer.Handlers;
using TcpServer.Models;

namespace ChatClientWPF.ViewModels
{
    [GenerateViewModel]
    public partial class MainViewModel : ViewModelBase
    {
        private string closecmd = "Close connection";

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
        ObservableCollection<string> userNames;

        [GenerateProperty]
        string message;



        TcpClient _client;
        StreamReader _reader;
        StreamWriter _writer;
        ChatJsonConverter chatJsonConverter = new ChatJsonConverter();

        [GenerateCommand]
        void Login() => Status = "User: " + userName;
        bool CanLogin() => !string.IsNullOrEmpty(userName);

        public MainViewModel()
        {
            ip = "192.168.1.105";
            port = 5050;
            userName = RandomeUserName();
            PrintInUI($"ip:{ip}:{port}");
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
                            _reader = new StreamReader(_client.GetStream());
                            _writer = new StreamWriter(_client.GetStream());
                            _writer.AutoFlush = true;

                            var cmd = chatJsonConverter.WriteToJson(new CommandMessage()
                            {
                                Command = "Login",
                                Argument = userName
                            });

                            //////////////////////////////////////////////////////////////////
                            //_writer.WriteLine(cmd);
                            SendBigSizeTCP(cmd);

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
                        //OpenFile.open();
                        //FileDialogs.Save();
                        FileDialogs.msg();
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
                    //_writer.WriteLine(msg);

                    var cmd = chatJsonConverter.WriteToJson(new CommandMessage()
                    {
                        Command = new CommandsHandler().CommandToString(TcpCommands.CloseConnection),
                        Argument = null
                    });

                    SendBigSizeTCP(cmd);
                    //SendBigSize(cmd);

                    PrintInUI(cmd);
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
                    //_writer.WriteLine(msg);

                    var cmd = chatJsonConverter.WriteToJson(new CommandMessage()
                    {
                        Command = "Message",
                        Argument = msg
                    });

                    SendBigSizeTCP(cmd);
                    //SendBigSize(cmd);

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

                    var cmd = chatJsonConverter.WriteToJson(new CommandMessage()
                    {
                        Command = "FileTransfer",
                        Argument = fileName
                    });
                    SendBigSize(cmd);
                    SendFileInByte(fileName);

                    PrintInUI(fileName);
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
                            //var line = _reader.ReadLine();
                            var line = ReceivingBigBufferTCP();
                            var cmd = chatJsonConverter.ReadFromJson(line);

                            if (!string.IsNullOrEmpty( line))
                            {
                                PrintInUI(cmd.Argument);
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

        private string RandomeUserName()
        {
            var names = new string[] { "Biser", "Tiser", "Ruser", "Niser", "Miser", "Cuser", "User", "Diser" };
            var r = new Random((int)DateTime.Now.Ticks);
            return names[r.Next(names.Length)];
        }
        private void PrintInUI(string message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Chat.Add(message);
            });
        }
        private void RunInUi(Action action)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                action();
            });

        }


        private void SendBigSize(string text)
        {
            var socket = _client.Client;
            byte[] data = Encoding.Default.GetBytes(text);
            socket.Send(BitConverter.GetBytes(data.Length), 0, 4, 0);
            socket.Send(data);
        }

        /// <summary>
        /// Отправка данных серверу
        /// </summary>
        /// <param name="text"></param>
        private void SendBigSizeTCP(string text)
        {
            // сообщение для отправки
            var message = text; // "Hello METANIT.COM";
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


        private void SendFileInByte(string fileName)
        {
            var stream = File.Open(fileName, FileMode.Open);
            //stream.Read



            //var socket = _client.Client;
            //byte[] data = Encoding.Default.GetBytes(text);
            //socket.Send(BitConverter.GetBytes(data.Length), 0, 4, 0);
            //socket.Send(data);
        }
    }
}
