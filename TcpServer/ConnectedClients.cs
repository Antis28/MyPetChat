using System.Net.Sockets;

namespace TcpServer
{
    internal class ConnectedClients
    {
        public TcpClient Client { get; set; }
        public string Name { get; set; }
        public ConnectedClients(TcpClient tcpClient, string name)
        {
            Client = tcpClient;
            Name = name;
        }
    }
}