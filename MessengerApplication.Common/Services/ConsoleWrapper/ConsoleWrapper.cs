using System;
using MessengerApplication.Common.Services.ConsoleWrapper.Interface;

namespace MessengerApplication.Common.Services.ConsoleWrapper
{
    /// <summary>
    /// Mock Console
    /// </summary>
    public class ConsoleWrapper : IConsole
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}