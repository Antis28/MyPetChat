using ChatMaui.Models;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatMaui.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        //----------------INotifyPropertyChanged----------------//
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //------------------------------------------------------//
        #endregion

        string userName = "";
        string ip = "";
        int port = 0;

        public ICommand AddCommand { get; set; }
       // public ICommand ConnectCommand { get; set; }
        public ObservableCollection<Person> People { get; } = new();

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
        public string message;



        TcpClient _client;
        StreamReader _reader;
        StreamWriter _writer;


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

        public ICommand ConnectCommand
        {
            get
            {
                return new Command(() =>
                {
                     Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            _client = new TcpClient();
                            _client.Connect(Ip, Port);
                            _reader = new StreamReader(_client.GetStream());
                            _writer = new StreamWriter(_client.GetStream());
                            _writer.AutoFlush = true;
                            _writer.WriteLine($"Login: {UserName}");
                            PrintInUI($"Подключение успешно!");
                        }
                        catch (Exception ex)
                        {
                            PrintInUI($"Ошибка: {ex.Message}");
                        }
                    });
                    // Если не подключен к серверу
                });
                //}, () => _client == null || _client?.Connected == false);
            }
        }
        public ICommand SendCommand
        {
            get
            {
                return new Command(() =>
                {
                    SendMsgAsync($"{UserName}: {Message}");

                });
                //}, () => _client?.Connected == true);
            }
        }

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
    }
}
