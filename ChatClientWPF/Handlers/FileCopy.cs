using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientWPF.Handlers
{
    internal class FileCopy
    {
        public delegate void Complet(bool ifComplete);
        public delegate void Progress(string message, int procent);

        /// <summary>
        /// Событие на завершение копирования файла
        /// </summary>
        public event Complet OnComplete;

        /// <summary>
        /// Событие во время копирования
        /// </summary>
        public event Progress OnProgress;

        // устанавливает размер порции или сколько за раз будет считано и записано информации.
        // Размер в байтах.
        // Значение этой переменной можно изменить во время работы программы.
        public int BufferLenght { get; set; }


        /// <summary>
        /// Копирование файла
        /// </summary>
        /// <param name="sourceFile">Путь к исходному файлу</param>
        /// <param name="destinationFile">Путь к целевому файлу</param>
        public void CopyFile(string sourceFile, string destinationFile)
        {
            try
            {
                CopyFromTo(sourceFile, destinationFile);
            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show("Возникла следующая ошибка при копировании:\n" + e.Message);
                //Отправляем сообщение что процесс копирования закончен неудачно
                if (OnComplete != null) OnComplete(false);
            }
        }
        public void CopyFile(FileStream sourceFile, NetworkStream destinationFile)
        {
            try
            {
                CopyFromToNetwork(sourceFile, destinationFile);
            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show("Возникла следующая ошибка при копировании:\n" + e.Message);
                //Отправляем сообщение что процесс копирования закончен неудачно
                if (OnComplete != null) OnComplete(false);
            }
        }

        private void CopyFromTo(string sourceFile, string destinationFile)
        {
            //Создаем буфер по размеру исходного файла
            //В буфер будем записывать информацию из файла
            Byte[] streamBuffer = new Byte[BufferLenght];

            //Общее количество считанных байт
            long totalBytesRead = 0;

            //Количество считываний
            //Используется для задания периода отправки сообщений
            int numReads = 0;

            //Готовим поток для исходного файла
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            {
                //Получаем длину исходного файла
                long sLenght = sourceStream.Length;
                //Готовим поток для целевого файла
                using (FileStream destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
                {
                    var isReadable = true;
                    //Читаем из буфера и записываем в целевой файл
                    while (isReadable) //Из цикла выйдем по окончанию копирования файла
                    {
                        isReadable = ReadFromBuffer(ref totalBytesRead, ref numReads, sourceStream, destinationStream);
                    }
                }
            }
            //Отправляем сообщение что процесс копирования закончен удачно
            if (OnComplete != null) OnComplete(true);
        }

        private void CopyFromToNetwork(FileStream sourceStream, NetworkStream destinationStream)
        {

            //Общее количество считанных байт
            long totalBytesRead = 0;

            //Количество считываний
            //Используется для задания периода отправки сообщений
            int numReads = 0;


            //Готовим поток для целевого файла
            var isReadable = true;
            //Читаем из буфера и записываем в целевой поток
            while (isReadable) //Из цикла выйдем по окончанию копирования файла
            {
                isReadable = ReadFromBufferNetwork(ref totalBytesRead, ref numReads, sourceStream, destinationStream);
            }

            //Отправляем сообщение что процесс копирования закончен удачно
            if (OnComplete != null) OnComplete(true);
        }

        private bool ReadFromBuffer(ref long totalBytesRead, ref int numReads, FileStream sourceStream, FileStream destinationStream)
        {
            //Получаем длину исходного файла
            long sLenght = sourceStream.Length;

            //Создаем буфер по размеру исходного файла
            //В буфер будем записывать информацию из файла
            Byte[] streamBuffer = new Byte[BufferLenght];

            //Увеличиваем на единицу количество считываний
            numReads++;
            //Записываем в буфер streamBuffer BufferLenght байт
            //bytesRead содержит количество записанных байт
            //это количество не может быть больше заданного BufferLenght
            int bytesRead = sourceStream.Read(streamBuffer, 0, BufferLenght);

            //Если ничего не было считано
            if (bytesRead == 0)
            {
                //Записываем информацию о процессе
                getInfo(sLenght, sLenght);
                //и выходим из цикла
                return false;
            }

            //Записываем данные буфера streamBuffer в целевой файл
            destinationStream.Write(streamBuffer, 0, bytesRead);
            //Для статистики запоминаем сколько уже байт записали
            totalBytesRead += bytesRead;

            // Если количество считываний кратно 10
            if (numReads % 10 == 0)
            {
                //Записываем информацию о процессе
                getInfo(totalBytesRead, sLenght);
            }

            //Если количество считанных байт меньше буфера
            //Значит это конец
            if (bytesRead < BufferLenght)
            {
                //Записываем информацию о процессе
                getInfo(totalBytesRead, sLenght);
                return false;
            }
            return true;
        }

        private bool ReadFromBufferNetwork(ref long totalBytesRead, ref int numReads, FileStream sourceStream, NetworkStream destinationStream)
        {

            BufferLenght = (int)sourceStream.Length / 100;
            //Создаем буфер по размеру исходного файла
            //В буфер будем записывать информацию из файла
            Byte[] streamBuffer = new Byte[BufferLenght];

            //Получаем длину исходного файла
            long sLenght = sourceStream.Length;

            //Увеличиваем на единицу количество считываний
            numReads++;
            //Записываем в буфер streamBuffer BufferLenght байт
            //bytesRead содержит количество записанных байт
            //это количество не может быть больше заданного BufferLenght
            int bytesRead = sourceStream.Read(streamBuffer, 0, BufferLenght);

            //Если ничего не было считано
            if (bytesRead == 0)
            {
                //Записываем информацию о процессе
                getInfo(sLenght, sLenght);
                //и выходим из цикла
                return false;
            }

            //Записываем данные буфера streamBuffer в целевой файл
            destinationStream.Write(streamBuffer, 0, bytesRead);
            //Для статистики запоминаем сколько уже байт записали
            totalBytesRead += bytesRead;

            // Если количество считываний кратно 10
            if (numReads % 10 == 0)
            {
                //Записываем информацию о процессе
                getInfo(totalBytesRead, sLenght);
            }

            //Если количество считанных байт меньше буфера
            //Значит это конец
            if (bytesRead < BufferLenght)
            {
                //Записываем информацию о процессе
                getInfo(totalBytesRead, sLenght);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Формирует сообщение для события прогресса
        /// </summary>
        /// <param name="totalBytesRead">Прочитано байт файла</param>
        /// <param name="sLenght">Длина файла</param>
        private void getInfo(long totalBytesRead, long sLenght)
        {
            //Формируем сообщение
            string message = string.Empty;
            double pctDone = (double)((double)totalBytesRead / (double)sLenght);
            var c = (int)(pctDone * 100);
            //message = string.Format("Считано: {0} из {1}. Всего {2}%",
            //    totalBytesRead,
            //    sLenght,
            //    (int)(pctDone * 100));
            message = $"Считано: {totalBytesRead} из {sLenght}. Всего {(int)(pctDone * 100)}%";
            //Отправляем сообщение подписавшимя на него
            if (OnProgress != null && c > 0) OnProgress(message, (int)(pctDone * 100));
        }
    }
}
