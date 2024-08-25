using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public enum TcpCommands
    {
        CloseConnection,
        Login,
        LoginSuccess,
        GetUsers,
        Message,
        FileTransfer,
        UpdateUserName,
    }
}
