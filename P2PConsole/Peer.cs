using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace P2PConsole
{
    internal class Peer
    {
        public enum HandshakeState
        {
            None,
            WaitingForResponse,
            Connected
        }

        private HandshakeState handshakeState = HandshakeState.None;


        public string Id { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public List<Peer> KnownPeers { get; set; }
        public Dictionary<string, FileInfo> AvailableFiles { get; set; }
        public UdpClient UdpClient { get; set; }
        public Thread ListenThread { get; set; }

        
        public Peer(string id, IPEndPoint endPoint)
        {
            Id = id;
            EndPoint = endPoint;
            KnownPeers = new List<Peer>();
            AvailableFiles = new Dictionary<string, FileInfo>();
            UdpClient = new UdpClient(EndPoint.Port);
            ListenThread = new Thread(ListenForPackets);
        }
        /// <summary>
        /// обрабатывать запросы на поиск файла и передачу файла:
        /// </summary>
        private void ListenForPackets()
        {
            while (true)
            {
                // Обработайте входящее сообщение, например, рукопожатие (handshake) или запрос на передачу файла
                UdpReceiveResult result = UdpClient.ReceiveAsync().Result;
                string message = Encoding.UTF8.GetString(result.Buffer);
                string[] data = message.Split('|');
                var package = new MessagePacket()
                {
                    Command = data[0],
                    PeerId = data[1],
                    PeerIp = data[2],
                    PeerPort = data[3],
                };

                if (package.Command == "HANDSHAKE")
                {
                    HandleHandshake(package);
                }
                else if (package.Command == "HANDSHAKE_ACK")
                {
                    HandleHandshakeAck(package);
                    
                }
                else if (package.Command == "ACK")
                {
                    HandleAck(package);

                }
                // Обработайте другие типы сообщений здесь

                if (package.Command == "SEARCH")
                {
                    HandleSearchRequest(data);
                }
                else if (package.Command == "FOUND")
                {
                    HandleFoundFile(data);
                }
                else if (package.Command == "REQUEST")
                {
                    HandleFileRequest(data);
                }
            }
        }

       

        /// <summary>
        /// для добавления известных узлов
        /// </summary>
        /// <param name="peer"></param>
        public void AddKnownPeer(Peer peer)
        {
            KnownPeers.Add(peer);
        }
        /// <summary>
        /// добавления известных узлов
        /// </summary>
        /// <param name="endPoint"></param>
        public void AddKnownPeer(IPEndPoint endPoint)
        {
            Peer newPeer = new Peer(Guid.NewGuid().ToString(), endPoint);
            KnownPeers.Add(newPeer);
        }
        /// <summary>
        /// Удаление известных узлов
        /// </summary>
        /// <param name="endPoint"></param>
        public void RemoveKnownPeer(IPEndPoint endPoint)
        {
            KnownPeers.RemoveAll(p => p.EndPoint.Equals(endPoint));
        }
        /// <summary>
        /// для добавления доступных файлов
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileInfo"></param>
        public void AddAvailableFile(string fileName, FileInfo fileInfo)
        {
            AvailableFiles.Add(fileName, fileInfo);
        }
        /// <summary>
        /// для отправки пакета данных по UDP:
        /// </summary>
        /// <param name="message"></param>
        /// <param name="endPoint"></param>
        public void SendPacket(string message, IPEndPoint endPoint)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            UdpClient.SendAsync(data, data.Length, endPoint);
        }
        /// <summary>
        /// будет отправлять запрос на поиск файла известным узлам:
        /// </summary>
        /// <param name="fileName"></param>
        public void SearchForFile(string fileName)
        {
            string message = $"SEARCH{fileName}";
            foreach (var peer in KnownPeers)
            {
                SendPacket(message, peer.EndPoint);
            }
        }
        /// <summary>
        /// будет обрабатывать входящие запросы на поиск файла и отправлять ответ,
        /// если файл доступен на этом узле:
        /// </summary>
        /// <param name="data"></param>
        private void HandleSearchRequest(string[] data)
        {
            string fileName = data[1];
            if (AvailableFiles.ContainsKey(fileName))
            {
                string message = $"FOUND{fileName}{AvailableFiles[fileName].Length}{AvailableFiles[fileName].FullName}";
                SendPacket(message, new IPEndPoint(IPAddress.Parse(data[0]), int.Parse(data[2])));
            }
        }
        /// <summary>
        ///  для отправки файлов:
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="recipientEndPoint"></param>
        public void SendFile(string filePath, IPEndPoint recipientEndPoint)
        {
            // Реализуйте разделение файла на пакеты и передачу их по сети
            // Отправьте файл по частям
            SendFileParts(filePath, recipientEndPoint);
        }
        /// <summary>
        ///  для остановки прослушивания входящих пакетов:
        /// </summary>
        public void Stop()
        {
            ListenThread.Abort();
            UdpClient.Close();
        }
        /// <summary>
        /// будет обрабатывать ответы на запрос поиска файла и
        /// добавлять найденные файлы в список известных файлов
        /// </summary>
        /// <param name="data"></param>
        private void HandleFoundFile(string[] data)
        {
            string fileName = data[1];
            long fileSize = long.Parse(data[2]);
            string filePath = data[3];

            // Добавьте найденный файл в список известных файлов
            AddKnownFile(fileName, fileSize, filePath);
        }
        /// <summary>
        ///  метод AddKnownFile, который будет добавлять найденный файл в список известных файлов
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileSize"></param>
        /// <param name="filePath"></param>
        public void AddKnownFile(string fileName, long fileSize, string filePath)
        {
            AvailableFiles.Add(fileName, new FileInfo(filePath));
        }
        /// <summary>
        /// будет отправлять запрос на передачу файла от узла, который имеет этот файл:
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="recipientEndPoint"></param>
        public void RequestFile(string fileName, IPEndPoint recipientEndPoint)
        {
            string message = $"REQUEST{fileName}{AvailableFiles[fileName].Length}{AvailableFiles[fileName].FullName}";
            SendPacket(message, recipientEndPoint);
        }

        /// <summary>
        /// будет обрабатывать запросы на передачу файла и отправлять файл в ответ:
        /// </summary>
        /// <param name="data"></param>
        private void HandleFileRequest(string[] data)
        {
            string fileName = data[1];
            long fileSize = long.Parse(data[2]);
            string filePath = data[3];

            // Отправьте файл по частям
            //SendFileParts(fileName, filePath, fileSize, new IPEndPoint(IPAddress.Parse(data[0]), int.Parse(data[4])));

            // Примите файл по частям
            ReceiveFileParts(fileName, fileSize, new IPEndPoint(IPAddress.Parse(data[0]), int.Parse(data[4])));
        }
        /// <summary>
        ///  отправлять файл по частям:
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <param name="fileSize"></param>
        /// <param name="recipientEndPoint"></param>
        private void SendFileParts(string fileName, string filePath, long fileSize, IPEndPoint recipientEndPoint)
        {
            byte[] buffer = new byte[1024];
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            long sentBytes = 0;
            while (sentBytes < fileSize)
            {
                int bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                SendPacket(Encoding.UTF8.GetString(buffer, 0, bytesRead), recipientEndPoint);
                sentBytes += bytesRead;
            }

            fileStream.Close();
        }

        /// <summary>
        ///  отправлять файл по частям:
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <param name="fileSize"></param>
        /// <param name="recipientEndPoint"></param>
        private void SendFileParts(string filePath, IPEndPoint recipientEndPoint)
        {
            byte[] buffer = new byte[1024];
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            long sentBytes = 0;
            while (sentBytes < fileStream.Length)
            {
                int bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                SendPacket(Encoding.UTF8.GetString(buffer, 0, bytesRead), recipientEndPoint);
                sentBytes += bytesRead;
            }

            fileStream.Close();
        }
        
        /// <summary>
        /// Принять
        /// файл по частям
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileSize"></param>
        /// <param name="senderEndPoint"></param>
        private void ReceiveFileParts(string fileName, long fileSize, IPEndPoint senderEndPoint)
        {
            byte[] buffer = new byte[1024];
            FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

            long receivedBytes = 0;
            while (receivedBytes < fileSize)
            {
                UdpReceiveResult result = UdpClient.ReceiveAsync().Result;
                byte[] data = result.Buffer;
                fileStream.Write(data, 0, data.Length);
                receivedBytes += data.Length;
            }

            fileStream.Close();
        }
       
        

        #region Handshake - методы для рукопожатия

        public void InitiateHandshake(IPEndPoint endPoint)
        {
            SendHandshake(endPoint);
        }
        /// <summary>
        /// для обработки входящих сообщений рукопожатия:
        /// </summary>
        /// <param name="packet"></param>
        private void HandleHandshake(MessagePacket packet)
        {
            Console.WriteLine($"Получен пакет от {packet.PeerIp}:{packet.PeerPort} - {packet.Command}");

            // Отправка SYN-ACK-пакета (ответ на SYN-пакет):
            string synAckData = $"HANDSHAKE_ACK|{Id}|{EndPoint.Address}|{EndPoint.Port}";
            SendPacket(synAckData, new IPEndPoint(IPAddress.Parse(packet.PeerIp), int.Parse(packet.PeerPort)));
            Console.WriteLine("SYN-ACK-пакет отправлен");
            handshakeState = HandshakeState.Connected;
        }

        /// <summary>
        /// Отправка ACK-пакета (подтверждение SYN-ACK-пакета):
        /// </summary>
        private void HandleHandshakeAck(MessagePacket packet)
        {
            handshakeState = HandshakeState.Connected;
            string handshakeMessage = $"ACK|{Id}|{EndPoint.Address}|{EndPoint.Port}";
            SendPacket(handshakeMessage, new IPEndPoint(IPAddress.Parse(packet.PeerIp), int.Parse(packet.PeerPort)));
            Console.WriteLine("ACK-пакет отправлен");
        }
        private void HandleAck(MessagePacket package)
        {
            handshakeState = HandshakeState.Connected;
        }

        /// <summary>
        ///  для отправки сообщения рукопожатия другому пиру:
        /// </summary>
        /// <param name="endPoint"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SendHandshake(IPEndPoint endPoint)
        {
            if (handshakeState != HandshakeState.None)
            {
                throw new InvalidOperationException("Handshake already in progress");
            }

            string handshakeMessage = $"HANDSHAKE|{Id}|{EndPoint.Address}|{EndPoint.Port}";
            SendPacket(handshakeMessage, endPoint);
            handshakeState = HandshakeState.WaitingForResponse;
            
            Console.WriteLine("SYN-пакет отправлен");
        }
        #endregion

    }
}
