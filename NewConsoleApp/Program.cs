using System;
using System.IO.Pipes;

string uniqueId;

// Check if the unique ID is passed as an argument
if (args.Length > 0)
{
    uniqueId = args[0];
    Console.WriteLine($"New Console Started with ID: {uniqueId}");
    while (true)
    {
        Console.Write("Enter a score: ");
        string input = Console.ReadLine();

        // Send the input to GameBoardConsole
        Task.Run(() => SendMessage(uniqueId, input));
    }
}
else
{
    Console.WriteLine("No unique ID provided.");
    // Generate a unique ID if none is provided
    uniqueId = Guid.NewGuid().ToString();
    Console.WriteLine($"Generated Unique ID: {uniqueId}");
}

static async Task SendMessage(string consoleId, string message)
{
    try
    {
        using (var client = new NamedPipeClientStream(".", "GameBoardPipe", PipeDirection.Out))
        {
            await client.ConnectAsync();
            using (var writer = new StreamWriter(client) { AutoFlush = true })
            {
                await writer.WriteLineAsync($"{consoleId}:{message}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending message: {ex.Message}");
    }
}