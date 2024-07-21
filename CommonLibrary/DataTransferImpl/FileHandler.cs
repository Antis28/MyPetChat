using CommonLibrary;
using CommonLibrary.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TcpServer.ViewModels.ClientHandlers
{
    public class FileHandler
    {
        TcpClient _client;
        ILogger _logger;

        public FileHandler(TcpClient tcpClient, ILogger logger)
        {
            _client = tcpClient;
            _logger = logger;
        }
        
        /// <summary>
        /// Принять файл
        /// </summary>
        /// <param name="savePath"></param>
        public void ReceiveFile(CommandMessage cmd)
        {
            string savePath = cmd.Argument;
            var stream = _client.GetStream();

            byte[] buf = new byte[65536];
            ReadBytes(sizeof(long), buf);
            long remainingLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buf, 0));

            using var file = File.Create(savePath);
            while (remainingLength > 0)
            {
                int lengthToRead = (int)Math.Min(remainingLength, buf.Length);
                ReadBytes(lengthToRead, buf);
                file.Write(buf, 0, lengthToRead);
                remainingLength -= lengthToRead;
            }
        }

        void ReadBytes(int howmuch, byte[] buf)
        {
            var stream = _client.GetStream();
            int readPos = 0;
            while (readPos < howmuch)
            {
                var actuallyRead = stream.Read(buf, readPos, howmuch - readPos);
                if (actuallyRead == 0)
                    throw new EndOfStreamException();
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
    }
}
