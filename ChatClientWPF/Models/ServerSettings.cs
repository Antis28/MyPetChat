﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft;

namespace ChatClientWPF.Models
{
    
    internal class ServerSettings
    {
        public string UserName { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}
