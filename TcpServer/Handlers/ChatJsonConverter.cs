using CommonLibrary;
using Newtonsoft.Json;
using System;
using System.Text;
using TcpServer.Models;

namespace TcpServer.Handlers
{
    public class ChatJsonConverter
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
        public string WriteToJson(object obj)
        {
            string serializedMsg = JsonConvert.SerializeObject(obj, Formatting.None);

            return serializedMsg;
        }

        public void test(string serializedMsg)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(serializedMsg);
        }
    }
}
