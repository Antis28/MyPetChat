using CommonLibrary.Interfaces;
using System;

namespace ChatHost
{
    internal class Logger : ILogger
    {
        public void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ShowMessage(message);
            Console.ResetColor();
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}