using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.NetWork
{
    public class IpVision
    {
        public static List<string> GetIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ips = new List<string>();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ips.Add(ip.ToString());
                }
            }
            return ips;
        }
    }
}
