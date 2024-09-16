using CommonLibrary.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TcpServer.ViewModels.ClientHandlers
{
    /// <summary>
    /// Обработка приема файла
    /// </summary>
    public class FileAcceptanceProcessing
    {
        private readonly TcpClient _client;
        private readonly ILogger _logger;
        private int percent;
        /// <summary>
        /// Событие на завершение копирования файла
        /// </summary>
        public event Action<bool, string> OnComplete;

        /// <summary>
        /// Событие во время копирования
        /// </summary>
        public event Action<string, int> OnProgress;


        public FileAcceptanceProcessing(TcpClient tcpClient, ILogger logger)
        {
            _client = tcpClient;
            _logger = logger;
        }


        /// <summary>
        /// Принять файл
        /// Где-то мы распознаем комманду на прием файла и переходим сюда
        /// </summary>
        /// <param name="cmd"></param>
        public void ReceiveFile(string savePath)
        {
            // формируем размер буфера для приема
            byte[] buf = new byte[65536];

            //Читаем из буфера
            ReadBytes(sizeof(long), buf);
            //Получаем размер всего файла
            long totalBytesRead = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buf, 0));
            //Получаем размер, котрый осталось для копирования
            long remainingLength = totalBytesRead;

            using var file = File.Create(savePath);
            percent = 0;
            while (remainingLength > 0)
            {
                // Длинна данных для чтения(что больше оставшиеся данные или буфер) 
                int lengthToRead = (int)Math.Min(remainingLength, buf.Length);

                ReadBytes(lengthToRead, buf);

                // Пишем байты из буфера в файл
                file.Write(buf, 0, lengthToRead);
                remainingLength -= lengthToRead;


                var curLen = totalBytesRead - remainingLength;
                //Записываем информацию о процессе
                getInfo(curLen, totalBytesRead);
            }

            OnComplete?.Invoke(true, savePath);
        }

        private void ReadBytes(int howmuch, byte[] buf)
        {
            var stream = _client.GetStream();
            int readPos = 0;
            while (readPos < howmuch)
            {
                var actuallyRead = stream.Read(buf, readPos, howmuch - readPos);
                if (actuallyRead == 0)// Мы не смоги что-либо прочитать, выдаем исключение
                { throw new EndOfStreamException(); }
                readPos += actuallyRead;
            }
        }


        /// <summary>
        /// Принять файл
        /// </summary>
        /// <param name="savePath"></param>
        public void ReceiveFile0(string savePath)
        {
            NetworkStream stream = _client.GetStream();
            FileStream fileStream = File.Create(savePath);
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
            }
            _logger.ShowMessage(savePath);
            fileStream.Close();
        }


        private void getInfo(long bytesRead, long totalLength)
        {
            //Формируем сообщение
            string message = string.Empty;
            double pctDone = (double)((double)bytesRead / (double)totalLength);
            var percentNow = (int)(pctDone * 100);
            // Выводить только кратно 10 процентам
            var per = percentNow % 10;
            if (percentNow == 0 || percent == percentNow || (percentNow % 10 != 0))
            {
                return;
            }
            percent = percentNow;
            if (totalLength / 1024 < 1000)
            {
                //Выводить в килобайтах
                message = $"Считано: {bytesRead / 1000}KB из {totalLength / 1000}KB. Всего {(int)(pctDone * 100)}%";
            }
            else
            //Выводить в мегабайтах
            { message = $"Считано: {bytesRead / 1000 / 1000}MB из {totalLength / 1000 / 1000}MB. Всего {(int)(pctDone * 100)}%"; }


            //Отправляем сообщение подписавшимся на него
            if (OnProgress != null && percent > 0) { OnProgress(message, percent); }
        }
    }
}
