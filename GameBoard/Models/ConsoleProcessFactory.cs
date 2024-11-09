using System;
using GameBoard.Interfaces;

namespace GameBoard.Models
{
    public static class ConsoleProcessFactory
    {
        public static IConsoleProcess CreateConsoleProcess(string type, string consoleId)
        {
            return type switch
            {
                "NewConsole" => new NewConsoleProcess(consoleId),
                "OldConsole" => new OldConsoleProcess(consoleId),
                _ => throw new ArgumentException("Invalid console type")
            };
        }
    }
}
