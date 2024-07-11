﻿using ChatClientWPF.Handlers;
using ChatClientWPF.Models;
using ChatClientWPF.Properties;
using DevExpress.Mvvm;
using DevExpress.Mvvm.CodeGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using TcpServer.Handlers;
using TcpServer.Models;

namespace ChatClientWPF.ViewModels
{
    [GenerateViewModel]
    public partial class MainViewModel : ViewModelBase
    {

        [GenerateProperty]
        public string ip;

        [GenerateProperty]
        public string userID;

        [GenerateProperty]
        public int port;
        [GenerateProperty]
        string status;

        [GenerateProperty]
        ObservableCollection<CommandMessage> chat = new();
        [GenerateProperty]
        ObservableCollection<ClientObject> userNames = new();

        //ObservableCollection<ClientObject> _userNames;

        //public ObservableCollection<ClientObject> UserNames        
        //{
        //    get { return _userNames; }
        //    set
        //    {
        //        if (_userNames != value)
        //        {
        //            _userNames = value;
        //            //RaisePropertyChanged(nameof(UserNames));
        //        }
        //    }
        //}

        [GenerateProperty]
        public int prgressCopyFile;

        [GenerateProperty]
        string message;

        string userName;
        public string UserName
        {
            get => userName;
            set
            {
                if (EqualityComparer<string>.Default.Equals(userName, value)) return;
                userName = value;

                if (_client != null) SendNewUserName();

                RaisePropertyChanged(nameof(UserName));
            }
        }

        public string _IsSelected = "1";
        public string IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (value == "Все")
                {

                }
                else
                {

                }

            }
        }

        public int _ItemIndex;
        public int ItemIndex
        {
            get
            {
                return _ItemIndex;
            }
            set
            {
                _ItemIndex = value;
            }
        }

        TcpClient _client;
        StreamReader _reader;
        StreamWriter _writer;
        ChatJsonConverter _chatJsonConverter = new ChatJsonConverter();
        CommandConverter _commandsHandler = new CommandConverter();
        DataTransfeHandler _dataTransfeHandler;

        ServerSettings settings;

        [GenerateCommand]
        void Login() => Status = "User: " + userName;
        bool CanLogin() => !string.IsNullOrEmpty(userName);

        public MainViewModel()
        {
            InitSettings();
            StartClient();
        }

        private void InitSettings()
        {
            if (JSaver.SettingExists())
            {
                settings = JSaver.LoadSetting<ServerSettings>();

            }
            else
            {
                settings = new ServerSettings()
                {
                    Ip = "192.168.1.105",
                    Port = 5050,
                    UserName = RandomeUserName(),
                };
                JSaver.Save(settings);
            }

            ip = settings.Ip;
            port = settings.Port;
            userName = settings.UserName;
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



                            _dataTransfeHandler = new DataTransfeHandler(_client);
                            _dataTransfeHandler.OnProgress += (x, y) => 
                            {
                                RunInUi(() =>
                                {
                                    chat.Add(new() { UserName = "System", Argument = x });
                                    prgressCopyFile = y;
                                });

                            };
                            _client.Connect(Ip, Port);
                            PrintInUI($"Подключение к ip:{ip}:{port}");
                            _reader = new StreamReader(_client.GetStream());
                            _writer = new StreamWriter(_client.GetStream());
                            _writer.AutoFlush = true;

                            Logining();
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

                    //JSaver.Save();
                    return SendMsgAsync(Message);

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

                    _dataTransfeHandler.SendBigSizeTCP(cmd);
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
                            var line = _dataTransfeHandler.ReceivingBigBufferTCP();
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
                Argument = string.Empty,
                IPAddress = "0",
                UserName = userName,
            });
            _dataTransfeHandler.SendBigSizeTCP(cmd);

            GetUsers();
        }
        private Task SendMsgAsync(string msg)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var cmdObj = new CommandMessage()
                    {
                        Command = _commandsHandler.CommandToString(TcpCommands.Message),
                        Argument = message,
                        UserName = UserName,
                        // заглушка для сообщения от клиента
                        IPAddress = "192.168.1.1",
                    };
                    PrintInUI(cmdObj);
                    Message = string.Empty;

                    var cmd = _chatJsonConverter.WriteToJson(cmdObj);
                    _dataTransfeHandler.SendBigSizeTCP(cmd);
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
                    PrintInUI($"Отправка файла: {fileName}");
                    _dataTransfeHandler.SendBigSizeTCP(cmdJs);
                    _dataTransfeHandler.SendBigSizeFileTCP2(fileName);

                    PrintInUI($"Файл отправлен: {fileName}");
                    Message = string.Empty;

                }
                catch (Exception ex)
                {
                    PrintInUI($"Ошибка: {ex.Message}");
                }
            });
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
                    PrintInUI(cmd);
                    break;
                case
                    TcpCommands.Login:
                    PrintInUI(cmd);
                    GetUsers();
                    break;
                case TcpCommands.LoginSuccess:
                    UserID = cmd.UserID;
                    PrintInUI(cmd);
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
        public Task GetUsersAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                GetUsers();
            });
        }
        /// <summary>
        /// Запрос списка пользователей
        /// </summary>
        private void GetUsers()
        {
            string cmd = NewCommand(TcpCommands.GetUsers);
            _dataTransfeHandler.SendBigSizeTCP(cmd);
        }

        private void SendNewUserName()
        {
            string cmd = NewCommand(TcpCommands.UpdateUserName);

            UserNames.FirstOrDefault(x => x.Id == UserID).UserName = UserName;


            _dataTransfeHandler.SendBigSizeTCP(cmd);
        }


        private string NewCommand(TcpCommands commandName)
        {
            return _chatJsonConverter.WriteToJson(new CommandMessage()
            {
                Command = _commandsHandler.CommandToString(commandName),
                UserName = UserName,
                Argument = null,
            });
        }

        #region Visualise
        private void VisualiseUserList(CommandMessage cmd)
        {
            var co = _chatJsonConverter.ReadFromJson<List<ClientObject>>(cmd.Argument);
            RunInUi(() =>
            {
                userNames.Clear();

                userNames.Add(new ClientObject("Все"));
                foreach (var item in co)
                {
                    userNames.Add(item);
                }

            });
        }

        private void PrintInUI(CommandMessage message)// string message)
        {
            RunInUi(() => Chat.Add(message));
        }
        private void PrintInUI(string message)
        {
            RunInUi(() => Chat.Add(new CommandMessage() { UserName = "System", Argument = message }));
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
