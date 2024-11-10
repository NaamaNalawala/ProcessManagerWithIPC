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

        public void InitializeConsoleData(IEnumerable<string> consoleIds)
        {
            foreach (var id in consoleIds)
            {
                consoleData[id] = "Waiting for input...";
            }
        }

        public void OnDataReceived(string consoleId, string data)
        {
            consoleData[consoleId] = data;
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

                // Sort the console data by score
                var sortedData = consoleData
                    .OrderByDescending(entry => int.TryParse(entry.Value, out int score) ? score : int.MinValue)
                    .ToList();

                foreach (var entry in sortedData)
                {
                    string consoleId = entry.Key;
                    string data = entry.Value;
                    string status = "Running";

                    // Determine the color based on console type (Old or New)
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

                    // Reset the console color to default
                    Console.ResetColor();
                }

                Console.WriteLine("-------------------------------------------------");
                Thread.Sleep(500);
            }
        }

    }

}
