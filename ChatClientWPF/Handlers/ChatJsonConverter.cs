using Newtonsoft.Json;
using System;
using System.Text;
using TcpServer.Models;

namespace TcpServer.Handlers
{
    internal class ChatJsonConverter
    {
        public CommandMessage ReadFromJson(String jsonString)
        {
            return JsonConvert.DeserializeObject<CommandMessage>(jsonString);
        }
        public string WriteToJson(CommandMessage commandMessage)
        {
            string serializedMsg = JsonConvert.SerializeObject(commandMessage, Formatting.None);
           
            return serializedMsg;
        }

        public void test(string serializedMsg)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(serializedMsg);
        }
    }
}
