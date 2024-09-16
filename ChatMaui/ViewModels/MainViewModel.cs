using ChatMaui.Models;
using CommonLibrary;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ChatMaui.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        //----------------INotifyPropertyChanged----------------//
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //------------------------------------------------------//
        #endregion

        private string userName = "";
        private string ip = "";
        private int port = 0;

        public ICommand AddCommand { get; set; }
        // public ICommand ConnectCommand { get; set; }
        public ObservableCollection<Person> People { get; } = new();

        private ChatJsonConverter _chatJsonConverter = new ChatJsonConverter();
        private CommandConverter _commandsHandler = new CommandConverter();
        private DataTransfeHandler _dataTransfeHandler;
        private ServerSettings settings;

        public string UserName
        {
            get => userName;
            set
            {
                if (userName != value)
                {
                    userName = value;
                    OnPropertyChanged();
                }

            }
        }
        public string Ip
        {
            get => ip;
            set
            {
                if (ip != value)
                {
                    ip = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Port
        {
            get => port;
            set
            {
                if (port != value)
                {
                    port = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Message { get; set; }
        public ObservableCollection<string> Chat { get; set; } = new();
        public ObservableCollection<string> UserNames { get; set; } = new();

        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;


        public MainViewModel()
        {
            // устанавливаем команду 

            UserName = RandomeseUserName();
            Ip = "192.168.1.105";
            Port = 5050;

            PrintInUI($"ip:{Ip}:{Port}");
            StartClient();
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
                            var line = _reader.ReadLine();
                            if (line != null)
                            {
                                PrintInUI(line);
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

        public ICommand ConnectCommand => new Command(() =>
                                                       {
                                                           Task.Factory.StartNew(() =>
                   {
                       try
                       {
                           _client = new TcpClient();



                           _dataTransfeHandler = new DataTransfeHandler(_client);
                           _dataTransfeHandler.OnProgress += (x, y) =>
                           {
                               PrintInUI($"Подписка на прогресс пересылки файла!");
                               //RunInUi(() =>
                               //{
                               //    chat.Add(new() { UserName = "System", Argument = x });
                               //    ProgressCopyFile = y;
                               //});

                           };
                           _dataTransfeHandler.OnComplete += (isSuccess, fileName) =>
                           {
                               if (isSuccess)
                               {
                                   PrintInUI($"Файл принят: {fileName} успешно.");
                               }
                               else
                               {
                                   PrintInUI($"Не удалось принять файл: {fileName}");
                               }
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
                                                       });//}, () => _client == null || _client?.Connected == false);
        public ICommand SendCommand => new Command(() =>
                                                    {
                                                        SendMsgAsync($"{UserName}: {Message}");

                                                    });//}, () => _client?.Connected == true);

        private Task SendMsgAsync(string msg)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    _writer.WriteLine(msg);
                    PrintInUI(msg);
                    Message = string.Empty;

                }
                catch (Exception ex)
                {
                    PrintInUI($"Ошибка: {ex.Message}");
                }
            });
        }
        private string RandomeseUserName()
        {

            var names = new string[] { "Biser", "Tiser", "Ruser", "Niser", "Miser", "Cuser", "User", "Diser" };
            var r = new Random((int)DateTime.Now.Ticks);
            return names[r.Next(names.Length)];
        }

        private void PrintInUI(string message)
        {
            Chat.Add(message);
            //App.Current.Dispatcher.Invoke(() =>
            //{
            //    Chat.Add(message);
            //});
        }

        private void Logining()
        {
            string hostIp = GetClientIp();

            var cmd = _chatJsonConverter.WriteToJson(new CommandMessage()
            {
                Command = "Login",
                Argument = string.Empty,
                IPAddress = hostIp,
                UserName = userName,
            });
            _dataTransfeHandler.SendBigSizeTCP(cmd);

            GetUsers();
        }
        private string GetClientIp()
        {
            var hostIp = "127.0.0.1";
            var host = Dns.GetHostEntry(Dns.GetHostName());


            foreach (var ip in host.AddressList)
            {
                var isInterNet = ip.AddressFamily == settings.AddressFamily;
                var IsStartWith192 = ip.ToString().StartsWith(settings.ClientIpStart);
                var IsEndNotWithOne = !ip.ToString().EndsWith(settings.ClientIpEnd);


                if (isInterNet && IsStartWith192 & IsEndNotWithOne)
                {
                    hostIp = ip.ToString();
                    break;
                }
            }

            return hostIp;
        }

        private void GetUsers()
        {
            string cmd = NewCommand(TcpCommands.GetUsers);
            _dataTransfeHandler.SendBigSizeTCP(cmd);
        }
        private string NewCommand(TcpCommands commandName)
        {
            return _chatJsonConverter.WriteToJson(new CommandMessage()
            {
                Command = _commandsHandler.CommandToString(commandName),
                UserName = UserName,
                Argument = null,
                IPAddress = null,
                Recipient = null,
                UserID = null,
            });
        }
    }
}
