

# **Game Board Application - Process Manager with Inter-Process Communication**

## **Overview**
The Game Board Application is a C# console-based application designed to track live scores from multiple gaming consoles. It supports both new and old console types, updates live scores, and displays the current status (`Running` or `Stopped`) of each console on the main game board.

## **Features**
- Supports both new and old gaming consoles.
- Provides a user-friendly UI to differentiate between old and new consoles.
- Tracks live scores in real time.
- Displays the current status of each console (`Running` or `Stopped`).
- Monitors console processes and updates the status if a process is manually stopped.
- Starts and manages multiple external processes.
- Facilitates real-time communication between processes using IPC.
- Organizes code effectively across multiple files and namespaces.
- Implements asynchronous operations to improve performance.

## **Prerequisites**
- .NET SDK (version 5.0 or higher) installed on your machine.
- Basic understanding of C# and console applications.

## **Installation**
1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/ProcessManagerWithIPC.git
   ```
2. **Navigate to the Project Directory:**
   ```bash
   cd ProcessManagerWithIPC
   ```
3. **Build the Solution:**
   - Open the solution in Visual Studio **or** run the following command:
     ```bash
     dotnet build
     ```

## **Running the Application**
1. **Start the Application:**
   - Navigate to the GameBoard project directory and run:
     ```bash
     dotnet run
     ```

   - Upon starting the application, you will see a welcome message and a prompt to enter the number of consoles:

     ```
     ***************************************
     *       Welcome to the Game Board App  *
     ***************************************
     Enter the number of NewConsoles to start (0-100):
     Enter the number of OldConsoles to start (0-100):
     ```

   - Enter the desired number of new and old consoles (between 0 and 100).
   - The application will start all the consoles and display a message indicating successful startup.

2. **View the Dashboard:**
   - The game board will display live scores and the status of each console:

     ```
     Console ID     | Score   | Status
     -----------------------------------
     Console_1      | 100     | Running
     OldConsole_1   | 85      | Stopped
     ```

3. **Interact with Consoles:**
   - **New Consoles:** Push data directly to the game board.
   - **Old Consoles:** Can be interacted with separately, and the game board will poll for their data periodically.

## **Technical Details**

### **Project Structure**
The Game Board Application consists of three projects:

1. **Main GameBoard Application:**
   - Manages live score updates from both new and old console apps.
   - Initializes, monitors, and displays the status and scores of all connected consoles.
   - Handles inter-process communication and status updates.

2. **NewConsole Application:**
   - Pushes live data directly to the GameBoard App.
   - Uses a push mechanism via Named Pipes for real-time score updates.

3. **OldConsole Application:**
   - Provides data using a polling mechanism.
   - The GameBoard App periodically polls the OldConsole App for the latest score updates.

### **Architecture and Design Patterns**
1. **Factory Design Pattern:**
   - Uses the `ConsoleProcessFactory` to create instances of different console processes (`NewConsoleProcess` and `OldConsoleProcess`).
   - Simplifies the creation logic based on console type, allowing easy scalability.

2. **Event-Driven Programming:**
   - Monitors console processes using the `Exited` event handler with `EnableRaisingEvents = true`.
   - Updates the status of each console automatically when the process exits.

3. **Inter-Process Communication (IPC):**
   - **Named Pipes:** Utilized for real-time communication between the game board and console processes.
   - **Push-Poll Mechanism:**
     - **Push:** New consoles push data updates directly to the game board.
     - **Polling:** The GameBoard App periodically polls old consoles for updates.

4. **Exception Handling and Error Logging:**
   - Basic exception handling is implemented to catch errors during process communication and data retrieval.

5. **Asynchronous Programming:**
   - Uses `async/await` for non-blocking I/O operations.
   - Manages multiple tasks concurrently to avoid overwhelming system resources.

## **Contact**
For any issues or support, please reach out:

- **Name:** Naama Taha Dhundhiyawala
- **Email:** [naama.nalawala@gmail.com](mailto:naama.nalawala@gmail.com)
