using CommonLibrary.Interfaces;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace TcpServer.ViewModels.ClientHandlers
{
    internal class DataTransfeHandler
    {
        ILogger _logger;
        TcpClient _client;

        public DataTransfeHandler(TcpClient client, ILogger logger) {
            _client = client;
            _logger = logger;
        }

        #region Отправка данных
        /// <summary>
        /// Отправка данных клиенту
        /// </summary>
        /// <param name="message">сообщение для отправки</param>
        public void SendBigSizeTCP(string message)
        {
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
    }
}
