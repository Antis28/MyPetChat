using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientWPF.Models
{
    public class ClientObject
    {

        public ClientObject(string name)
        {
            UserName = name;
            Id = "-1";
        }

        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
