using GameBoard.Interfaces;
using GameBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard.Services
{

    public class OldConsolePoller
    {
        private readonly List<OldConsoleProcess> oldConsoles;
        private readonly IConsoleDataListener dataListener;
        private bool _isPolling;

        public OldConsolePoller(List<OldConsoleProcess> oldConsoles, IConsoleDataListener dataListener)
        {
            this.oldConsoles = oldConsoles;
            this.dataListener = dataListener;
            _isPolling = true;
        }

        public void StartPolling()
        {
            _isPolling = true;
            var pollingThread = new Thread(async () =>
            {
                while (_isPolling)
                {
                    foreach (var console in oldConsoles)
                    {
                        // Skip polling if the console status is "Stopped"
                        if (console.Status == "Stopped")
                        {
                            continue;
                        }

                        var score = await console.GetDataAsync();
                        dataListener.OnDataReceived(console.ConsoleId, score);
                    }

                    // Sleep for a short interval before polling again
                    Thread.Sleep(1000);
                }
            });

            pollingThread.Start();
        }
        public void StopPolling()
        {
            _isPolling = false;
        }
    }

}
