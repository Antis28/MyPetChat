using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft;
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
