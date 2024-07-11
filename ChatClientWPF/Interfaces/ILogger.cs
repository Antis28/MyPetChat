using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientWPF.Interfaces
{
    public interface ILogger
    {
        void ShowMessage(string message);
        void ShowError(string message);
    }
}
