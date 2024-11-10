using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using OldConsoleApp.Interfaces;

namespace OldConsoleApp.Services
{
    public class NamedPipeServer : IPipeServer
    {
        private readonly string _pipeName;
        private readonly IScoreData _scoreData;

        public NamedPipeServer(string pipeName, IScoreData scoreData)
        {
            _pipeName = pipeName;
            _scoreData = scoreData;
        }

        public async Task StartAsync()
        {
            while (true)
            {
                try
                {
                    using var server = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                    //Logger.Log($"Waiting for connection: {_pipeName}");

                    await server.WaitForConnectionAsync();
                    //Logger.Log($"Connection established: {_pipeName}");

                    using var writer = new StreamWriter(server) { AutoFlush = true };

                    // Write the current score data to the stream
                    string dataToWrite = _scoreData.CurrentData;
                    await writer.WriteLineAsync(dataToWrite);
                    //Logger.Log($"Wrote: {dataToWrite}");

                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error in NamedPipeServer: {ex.Message}");
                }
            }
        }
    }
}
