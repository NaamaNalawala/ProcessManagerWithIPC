using OldConsoleApp.Interfaces;

namespace OldConsoleApp.Services
{
    // Score data implementation
    public class ScoreData : IScoreData
    {
        // Using a lock to ensure thread safety
        private readonly object _dataLock = new object();
        private string _currentData = "0";

        public string CurrentData
        {
            get
            {
                lock (_dataLock)
                {
                    return _currentData;
                }
            }
            set
            {
                lock (_dataLock)
                {
                    _currentData = value;
                }
            }
        }
    }
}
