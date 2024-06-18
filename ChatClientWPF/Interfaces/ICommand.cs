using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.Interfaces
{
    public interface ICommandMessage
    {
        string Command { get; set; }
        string Argument { get; set; }
    }
}
