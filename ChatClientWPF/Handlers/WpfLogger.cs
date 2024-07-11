using ChatClientWPF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientWPF.Handlers
{
    internal class WpfLogger : ILogger
    {
        public WpfLogger()
        {
            
        }

        public void ShowError(string message)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}
