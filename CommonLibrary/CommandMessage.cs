using CommonLibrary.Interfaces;

namespace CommonLibrary
{
    public class CommandMessage : ICommandMessage
    {
        public string Command { get; set; }
        public string UserName { get; set; }
        public string UserID { get; set; }
        public string IPAddress { get; set; }
        public string Argument { get; set; }
        // Личное сообщение, получатель
        public string Recipient { get; set; }
    }
}
