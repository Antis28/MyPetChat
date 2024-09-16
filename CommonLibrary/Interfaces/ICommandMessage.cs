namespace CommonLibrary.Interfaces
{
    public interface ICommandMessage
    {
        public string Command { get; set; }
        public string Argument { get; set; }
        public string UserName { get; set; }
        public string UserID { get; set; }
        public string IPAddress { get; set; }
        // Личное сообщение, получатель
        public string Recipient { get; set; }
    }
}
