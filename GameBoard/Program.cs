using GameBoard;
using System.Diagnostics;
using System.IO.Pipes;

Console.Write("Enter the number of NewConsoles to start (1-100): ");
string newConsoleInput = Console.ReadLine();
int numberOfNewConsoles;

Console.Write("Enter the number of OldConsoles to start (1-100): ");
string oldConsoleInput = Console.ReadLine();
int numberOfOldConsoles;

if (int.TryParse(newConsoleInput, out numberOfNewConsoles) && numberOfNewConsoles >= 1 && numberOfNewConsoles <= 100 &&
    int.TryParse(oldConsoleInput, out numberOfOldConsoles) && numberOfOldConsoles >= 1 && numberOfOldConsoles <= 100)
{
    StartConsoles("NewConsoleApp.exe", "Console_", numberOfNewConsoles);
    StartConsoles("OldConsoleApp.exe", "OldConsole_", numberOfOldConsoles);

    Console.WriteLine($"Successfully started {numberOfNewConsoles} NewConsoles and {numberOfOldConsoles} OldConsoles.");

    Task.Run(() => ListenForUpdates());
    StartPollingOldConsoles(numberOfOldConsoles);

    DisplayGameBoard();
}
else
{
    Console.WriteLine("Invalid input. Please enter numbers between 1 and 100.");
}

Console.WriteLine("Press any key to exit.");
Console.ReadKey();
static void StartConsoles(string executablePath, string prefix, int count)
{
    for (int i = 1; i <= count; i++)
    {
        string uniqueId = $"{prefix}{i}";
        Globals.consoleData[uniqueId] = "Waiting for input...";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = executablePath,
            Arguments = uniqueId,
            UseShellExecute = true,
            CreateNoWindow = false
        };

        Process.Start(psi);
        Console.WriteLine($"Started {prefix} with ID: {uniqueId}");
    }
}
static async void ListenForUpdates()
{
    while (true)
    {
        using (var server = new NamedPipeServerStream("GameBoardPipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
        {
            await server.WaitForConnectionAsync();
            using (var reader = new StreamReader(server))
            {
                string message = await reader.ReadLineAsync();
                if (message != null)
                {
                    var parts = message.Split(':');
                    string consoleId = parts[0];
                    string value = parts[1];

                    // Update the dictionary with the new value
                    lock (Globals.consoleData)
                    {
                        if (Globals.consoleData.ContainsKey(consoleId))
                        {
                            Globals.consoleData[consoleId] = value;
                        }
                    }
                }
            }
        }
    }
}

static void StartPollingOldConsoles(int numberOfOldConsoles)
{
    Timer timer = new Timer(_ =>
    {
        for (int i = 1; i <= numberOfOldConsoles; i++)
        {
            string consoleId = $"OldConsole_{i}";
            Task.Run(async () =>
            {
                string value = await GetOldConsoleData(consoleId);
                lock (Globals.consoleData)
                {
                    Globals.consoleData[consoleId] = value;
                }
            });
        }
    }, null, 0, 1000);
}

static async Task<string> GetOldConsoleData(string consoleId)
{
    try
    {
        using (var client = new NamedPipeClientStream(".", consoleId, PipeDirection.In))
        {
            await client.ConnectAsync(1000);
            using (var reader = new StreamReader(client))
            {
                return await reader.ReadLineAsync() ?? "No data";
            }
        }
    }
    catch
    {
        return "Error fetching data";
    }
}
static void DisplayGameBoard()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("GameBoard Console:");
        foreach (var entry in Globals.consoleData)
        {
            Console.WriteLine($"{entry.Key} - {entry.Value}");
        }
        Thread.Sleep(500);
    }
}