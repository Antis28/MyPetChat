﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.CodeGenerators;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Threading;
using System.Diagnostics;

namespace ChatClientWPF.ViewModels
{
    [GenerateViewModel]
    public partial class MainViewModel //: ViewModelBase
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
        ObservableCollection<string> userNames;
        
        [GenerateProperty]
        string message;


        TcpClient _client;
        StreamReader _reader;
        StreamWriter _writer;

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
                            _writer.WriteLine($"Login: {userName}");
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

        public AsyncCommand SendCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            _writer.WriteLine($"{userName}: {Message}");
                            Message = string.Empty;
                        }
                        catch (Exception ex)
                        {
                            PrintInUI($"Ошибка: {ex.Message}");
                        }
                    });

                }, () => _client.Connected, !string.IsNullOrWhiteSpace(Message));
            }
        }

        private void StartClient()
        {
            
            Task.Factory.StartNew(async () => {
                while (true)
                {
                    try
                    {
                        if ((_client?.Connected) == true)
                        {
                            PrintInUI($"Подключение успешно!");
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
    }
}
