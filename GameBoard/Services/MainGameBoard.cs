using GameBoard.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard.Services
{

    public class MainGameBoard : IConsoleDataListener
    {
        private readonly ConcurrentDictionary<string, string> consoleData = new();
        private readonly Dictionary<string, IConsoleProcess> _consoleProcesses = new();


        public void InitializeConsoleData(IEnumerable<string> consoleIds)
        {
            foreach (var id in consoleIds)
            {
                consoleData[id] = "0";
            }
        }

        public void RegisterConsoleProcess(IConsoleProcess consoleProcess)
        {
            _consoleProcesses[consoleProcess.ConsoleId] = consoleProcess;
        }

        public void OnDataReceived(string consoleId, string value)
        {
            if (consoleData.ContainsKey(consoleId))
            {
                consoleData[consoleId] = value;
            }
        }
        public void Display()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("************************************");
                Console.WriteLine("*         GameBoard Console        *");
                Console.WriteLine("************************************");
                Console.WriteLine("| Console ID     | Data              | Status   |");
                Console.WriteLine("-------------------------------------------------");

                // Sort the console data by score before displaying
                var sortedData = consoleData
                    .OrderByDescending(entry => int.TryParse(entry.Value, out int score) ? score : int.MinValue)
                    .ToList();

                foreach (var entry in sortedData)
                {
                    string consoleId = entry.Key;
                    string data = entry.Value;
                    var status = _consoleProcesses.ContainsKey(consoleId)
                    ? _consoleProcesses[consoleId].Status
                    : "Unknown";

                    if (consoleId.StartsWith("OldConsole"))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else if (consoleId.StartsWith("Console"))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }

                    // Display in a tabular format
                    Console.WriteLine($"| {consoleId,-13} | {data,-16} | {status,-8} |");

                    Console.ResetColor();
                }

                Console.WriteLine("-------------------------------------------------");
                Thread.Sleep(500);
            }
        }

    }

}
