using CommonLibrary.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace CommonLibrary
{
    public class DataTransfeHandler
    {
        ILogger _logger;
        int copyProgress;
        TcpClient _client;

        public event Progress OnProgress;

        public DataTransfeHandler(TcpClient client)
        {
          //  _logger = logger;
            _client = client;            
        }

        public DataTransfeHandler(TcpClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }


        #region Отправка данных
        public void SendBigSizeFileTCP2(string fileName)
        {
            var fileCopyInstance = new FileCopy();

            // получаем NetworkStream для взаимодействия с принимающей стороной
            var netDestinationStream = _client.GetStream();
            //Готовим поток для исходного файла
            using (FileStream fileSourceStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                //Получаем длину исходного файла
                long sLenght = fileSourceStream.Length;
                
                byte[] size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(sLenght));
                netDestinationStream.Write(size, 0, size.Length);

                fileCopyInstance.OnProgress += FileCopyInstance_OnProgress;
                fileCopyInstance.BufferLenght = 4096;
                fileCopyInstance.CopyFile(fileSourceStream, netDestinationStream);
                
                
            }
            
            //using (FileStream fileStream = File.OpenRead(fileName))
            //{
            //    // byte[] buffer = new byte[4096];
            //    //int bytesRead;
            //    var length = fileStream.Length;
            //    byte[] size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));
            //    stream.Write(size, 0, size.Length);
            //    // отправляем данные
            //    fileStream.CopyTo(stream);
            //    //stream.Write(data, 0, data.Length);
            //}


            //var f1 = size1;
            //var f2 = data.Length;
            //// определяем размер данных
            //byte[] size = BitConverter.GetBytes(data.Length);
            // отправляем размер данных
            //stream.Write(size, 0, 4);
            // отправляем данные
            // stream.Write(data, 0, data.Length);
        }

        private void FileCopyInstance_OnProgress(string message, int procent)
        {
            if (OnProgress != null) OnProgress(message, procent);
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

        #endregion
       

        #region Прием данных
        /// <summary>
        /// Прием сырых данных от клиента
        /// </summary>
        /// <returns></returns>
        public Tuple<byte[], int> ReceivingBigBufferRawDataTCP()
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
        /// Прием строковых данных от клиента
        /// </summary>
        /// <returns></returns>
        public string ReceivingBigBufferTCP()
        {
            var (data, byteLength) = ReceivingBigBufferRawDataTCP();
            var message = Encoding.UTF8.GetString(data, 0, byteLength);
            return message;
        }
        #endregion




        #region Отправка данных 2

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
        public void SendBigSize(string text)
        {
            var socket = _client.Client;
            byte[] data = Encoding.Default.GetBytes(text);
            socket.Send(BitConverter.GetBytes(data.Length), 0, 4, 0);
            socket.Send(data);
        }
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
        /// Отправка данных клиенту
        /// </summary>
        /// <param name="message">сообщение для отправки</param>
        public void SendBigSizeTCP(byte[] data, int size)
        {
            // получаем NetworkStream для взаимодействия с сервером
            var stream = _client.GetStream();
            // считыванием строку в массив байт
            //byte[] data = Encoding.UTF8.GetBytes(message);
            //// определяем размер данных
            var f1 = size;
            var f2 = data.Length;
            byte[] sizeInByte = BitConverter.GetBytes(data.Length);
            // отправляем размер данных
            stream.Write(sizeInByte, 0, 4);
            // отправляем данные
            stream.Write(data, 0, data.Length);
        }
        /// <summary>
        /// Server Buff handler
        /// Прием данных от клиента
        /// </summary>
        /// <param name="connectedClient"></param>
        /// <returns></returns>
        public string HandlerBigBuffer()
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

        #endregion
    }
}
