using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Interfaces;

namespace TcpServer.Models
{
    public class CommandMessage : ICommandMessage
    {
        public string Command { get; set; }
        public string UserName { get; set; }
        public string UserID { get; set; }        
        public string IPAddress { get;  set; }
        public string Argument { get; set; }
    }
}
