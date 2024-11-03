using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PConsole
{
    internal class MessagePacket
    {
        public string Command { get; set; }
        public string PeerId { get; set; }

        public string PeerIp { get; set; }
        public string PeerPort { get; set; }
        public MessagePacket() { }
    }
}
