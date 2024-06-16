using System.Net.Sockets;

namespace TcpServer
{
    internal class ConnectedClient
    {
        static int nextId = 1;
        public int ID { get; set; }
        public TcpClient Client { get; set; }
        public string Name { get; set; }
        public ConnectedClient(TcpClient tcpClient, string name)
        {
            Client = tcpClient;
            Name = name;
            ID = nextId++;
        }
    }
}