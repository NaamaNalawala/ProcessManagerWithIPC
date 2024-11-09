using GameBoard.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard.Services
{

    public class NamedPipeListener
    {
        private readonly string pipeName;
        private readonly IConsoleDataListener dataListener;

        public NamedPipeListener(string pipeName, IConsoleDataListener dataListener)
        {
            this.pipeName = pipeName;
            this.dataListener = dataListener;
        }

        public async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var server = new NamedPipeServerStream(pipeName, PipeDirection.In, NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Message, PipeOptions.Asynchronous);

                try
                {
                    await server.WaitForConnectionAsync(cancellationToken);

                    _ = Task.Run(async () =>
                    {
                        using (server)
                        {
                            try
                            {
                                using var reader = new StreamReader(server);
                                var message = await reader.ReadLineAsync();
                                if (!string.IsNullOrWhiteSpace(message))
                                {
                                    var parts = message.Split(':');
                                    if (parts.Length == 2)
                                    {
                                        string consoleId = parts[0];
                                        string value = parts[1];
                                        dataListener.OnDataReceived(consoleId, value);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in NamedPipeListener Task: {ex.Message}");
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in NamedPipeListener: {ex.Message}");
                }
            }
        }
    }

}
