using Newtonsoft.Json;

namespace TcpServer.Models
{
    internal class ChatCommands
    {
        [JsonProperty("Login")]
        public string Login { get; set; }
        [JsonProperty("CloseConection")]
        public string CloseConection { get; set; }
        [JsonProperty("GetUsers")]
        public string GetUsers { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }
        [JsonProperty("FileTransfer")]
        public string FileTransfer { get; set; }
        [JsonProperty("UpdateUserName")]
        public string UpdateUserName { get; set; }
        [JsonProperty("LoginSuccess")]
        public string LoginSuccess { get; internal set; }
    }
}
