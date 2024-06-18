using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Models;
using Newtonsoft.Json;

namespace TcpServer.Handlers
{
    internal class ChatJsonConverter
    {
        public void ReadFromJson(String jsonString)
        {
            CommandMessage deserializedMessage;
            try { deserializedMessage = JsonConvert.DeserializeObject<CommandMessage>(jsonString); }
            catch (Exception e)
            {
                return;
            }
        }
        public void WriteToJson(CommandMessage commandMessage)
        {

            string serializedMsg = JsonConvert.SerializeObject(commandMessage, Formatting.Indented);
            byte[] messageBytes = Encoding.UTF8.GetBytes(serializedMsg);
        }

        public void test()
        {
            
        }
    }
}
