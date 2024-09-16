namespace CommonLibrary.Interfaces
{
    public interface ILogger
    {
        void ShowMessage(string message);
        void ShowError(string message);
    }
}
