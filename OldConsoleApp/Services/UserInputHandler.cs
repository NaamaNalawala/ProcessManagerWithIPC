using System;
using System.Threading.Tasks;
using OldConsoleApp.Interfaces;

namespace OldConsoleApp.Services
{
    // User input handler implementation
    public class UserInputHandler : IUserInputHandler
    {
        private readonly IScoreData _scoreData;

        public UserInputHandler(IScoreData scoreData)
        {
            _scoreData = scoreData;
        }

        public async Task StartAsync()
        {
            while (true)
            {
                Console.Write("Enter a score: ");
                var input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    _scoreData.CurrentData = input;
                    Logger.Log($"Updated score to: {_scoreData.CurrentData}");
                }
                else
                {
                    Console.WriteLine("Input cannot be empty. Please try again.");
                }

                await Task.Yield(); // Yield control to allow other tasks to run
            }
        }
    }
}
