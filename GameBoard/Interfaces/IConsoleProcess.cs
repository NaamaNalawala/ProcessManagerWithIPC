using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard.Interfaces
{
    public interface IConsoleProcess
    {
        string ConsoleId { get; }
        string Status { get; set; }
        Task StartAsync();
    }
}
