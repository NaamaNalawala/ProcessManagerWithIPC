using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard.Interfaces
{
    public interface IConsoleDataListener
    {
        void OnDataReceived(string consoleId, string data);
    }
}
