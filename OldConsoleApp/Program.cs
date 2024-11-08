using OldConsoleApp;
using System.IO.Pipes;


if (args.Length == 0)
{
    Console.WriteLine("Please provide a Console ID.");
    return;
}

string consoleId = args[0];
Console.WriteLine($"Old Console started with ID: {consoleId}");
Task.Run(() => StartNamedPipeServer(consoleId));

while (true)
{
    Console.Write("Enter a score: ");
    Globals.currentData = Console.ReadLine();
}

static async void StartNamedPipeServer(string consoleId)
{
    while (true)
    {
        try
        {
            using (var server = new NamedPipeServerStream(consoleId, PipeDirection.Out, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                await server.WaitForConnectionAsync();
                using (var writer = new StreamWriter(server) { AutoFlush = true })
                {
                    await writer.WriteLineAsync(Globals.currentData);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error responding to polling: {ex.Message}");
        }
    }
}