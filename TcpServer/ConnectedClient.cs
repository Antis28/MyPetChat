using System.Net.Sockets;

namespace TcpServer
{
    internal class ConnectedClient
    {
        public TcpClient Client { get; set; }
        public string Name { get; set; }
        public ConnectedClient(TcpClient tcpClient, string name)
        {
            Client = tcpClient;
            Name = name;
        }
    }
}