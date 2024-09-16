using System.Net.Sockets;

namespace CommonLibrary
{

    public class ServerSettings
    {
        public int ID { get; set; }

        public string UserName { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }

        public string ClientIpStart { get; set; }
        public string ClientIpEnd { get; set; }
        public AddressFamily AddressFamily { get; set; }
    }
}
