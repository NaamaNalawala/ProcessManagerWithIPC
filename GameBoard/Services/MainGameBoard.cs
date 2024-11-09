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
                Console.WriteLine("GameBoard Console:");
                foreach (var entry in consoleData)
                {
                    Console.WriteLine($"{entry.Key} - {entry.Value}");
                }
                Thread.Sleep(500);
            }
        }
    }

}
