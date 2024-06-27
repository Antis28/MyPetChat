using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.Interfaces
{
    public interface ICommandMessage
    {
        public string Command { get; set; }
        public string UserName { get; set; }
        public string UserID { get; set; }
        public string IPAddress { get; set; }
        public string Argument { get; set; }
    }
}
