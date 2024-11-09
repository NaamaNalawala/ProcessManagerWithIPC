using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

string currentData = "0";

if (args.Length == 0)
{
    Console.WriteLine("Please provide a Console ID.");
    return;
}

string consoleId = args[0];
Console.WriteLine($"Old Console started with ID: {consoleId}");

// Start the named pipe server
_ = Task.Run(() => StartNamedPipeServer(consoleId));

// Main loop to accept user input
while (true)
{
    Console.Write("Enter a score: ");
    var input = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(input))
    {
        currentData = input;
        Console.WriteLine($"Updated score: {currentData}");
    }
}

async Task StartNamedPipeServer(string consoleId)
{
    while (true)
    {
        try
        {
            using (var server = new NamedPipeServerStream(consoleId, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                Console.WriteLine($"Waiting for connection: {consoleId}");

                // Wait for a client connection
                await server.WaitForConnectionAsync();
                Console.WriteLine($"Connection established: {consoleId}");

                using (var writer = new StreamWriter(server) { AutoFlush = true })
                {
                    // Write the current data to the stream
                    string dataToWrite = currentData;
                    await writer.WriteLineAsync(dataToWrite);
                    Console.WriteLine($"Wrote: {dataToWrite}");
                    await Task.Delay(1000);
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in StartNamedPipeServer: {ex.Message}");
        }
    }
}
