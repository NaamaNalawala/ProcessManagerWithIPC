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

        public OldConsolePoller(List<OldConsoleProcess> oldConsoles, IConsoleDataListener dataListener)
        {
            this.oldConsoles = oldConsoles;
            this.dataListener = dataListener;
        }

        public void StartPolling()
        {
            foreach (var console in oldConsoles)
            {
                int pollingInterval = 5000;
                var timer = new Timer(async _ =>
                {
                    var data = await console.GetDataAsync();
                    dataListener.OnDataReceived(console.ConsoleId, data);
                }, null, 0, pollingInterval);
            }
        }
    }

}
