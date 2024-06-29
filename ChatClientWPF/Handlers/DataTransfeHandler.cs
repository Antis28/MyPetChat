using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientWPF.Handlers
{
    internal class DataTransfeHandler
    {
        //ILogger _logger;
        TcpClient _client;
        public DataTransfeHandler(TcpClient client)
        {
            _client = client;            
        }
        /// <summary>
        /// Прием данных от сервера
        /// </summary>
        /// <returns></returns>
        public string ReceivingBigBufferTCP()
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

        #region Отправка даных
        public void SendFile(string fileName)
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
        /// Отправка данных
        /// </summary>
        /// <param name="message">сообщение для отправки</param>
        public void SendBigSizeFileTCP(string fileName)
        {
            // получаем NetworkStream для взаимодействия с принимающей стороной
            var stream = _client.GetStream();
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                // byte[] buffer = new byte[4096];
                //int bytesRead;
                var length = fileStream.Length;
                byte[] size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));
                stream.Write(size, 0, size.Length);
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
        /// <summary>
        /// Отправка данных серверу
        /// </summary>
        /// <param name="cmdJs"></param>
        public void SendBigSizeTCP(string cmdJs)
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

        public void SendBigSize(string text)
        {
            var socket = _client.Client;
            byte[] data = Encoding.Default.GetBytes(text);
            socket.Send(BitConverter.GetBytes(data.Length), 0, 4, 0);
            socket.Send(data);
        }
        #endregion
    }
}
