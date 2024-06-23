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
        public T ReadFromJson<T>(String jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
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
