using System;
using System.IO;
using System.Net.Sockets;

namespace CommonLibrary
{
    public delegate void Complete(bool isSuccess);
    public delegate void Progress(string message, int percent);

    /// <summary>
    /// Реализация отправки файла с индикатором прогреса и завершения
    /// </summary>
    public class FileSendingProcessing
    {
        private int percent;

        /// <summary>
        /// Событие на завершение копирования файла
        /// </summary>
        public event Action<bool, string> OnComplete;

        /// <summary>
        /// Событие во время копирования
        /// </summary>
        public event Action<string, int> OnProgress;

        // устанавливает размер порции или сколько за раз будет считано и записано информации.
        // Размер в байтах.
        // Значение этой переменной можно изменить во время работы программы.
        public int BufferLenght { get; set; }

        public FileSendingProcessing()
        {
            BufferLenght = 8192;
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
                if (OnComplete != null)
                {
                    OnComplete(false, sourceFile.Name);
                }
            }
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
            if (OnComplete != null)
            {
                OnComplete(true, sourceStream.Name);
            }
        }

        private bool ReadFromBufferNetwork(ref long totalBytesRead, ref int numReads, FileStream sourceStream, NetworkStream destinationStream)
        {
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
                //и выходим из цикла чтения
                return false;
            }

            //Записываем данные буфера streamBuffer в целевой поток
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
            var percentNow = (int)(pctDone * 100);

            // Выводить только кратно 10 процентам            
            if (percentNow == 0 || percent == percentNow || (percentNow % 10 != 0))
            {
                return;
            }
            percent = percentNow;
            if (sLenght / 1024 < 1000)
            {
                //Выводить в килобайтах
                message = $"Считано: {totalBytesRead / 1000}KB из {sLenght / 1000}KB. Всего {(int)(pctDone * 100)}%";
            }
            else
            {
                //Выводить в мегабайтах
                message = $"Считано: {totalBytesRead / 1000 / 1000}MB из {sLenght / 1000 / 1000}MB. Всего {(int)(pctDone * 100)}%";
            }


            //Отправляем сообщение подписавшимся на него
            if (OnProgress != null)
            {
                OnProgress(message, percent);
            }
        }







        #region Другие способы копирования файлов (в файловом потоке)
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
                if (OnComplete != null)
                {
                    OnComplete(false, sourceFile);
                }
            }
            percent = 0;
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
            if (OnComplete != null)
            {
                OnComplete(true, sourceFile);
            }
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
        #endregion
    }
}
