using System;

namespace Agridea.Threading
{
    [Serializable]
    public class JobProgressEventArgs : EventArgs
    {
        public JobProgressEventArgs(string progressRate)
        {
            ProgressRate = progressRate;
        }
        public string ProgressRate { get; private set; }
    }
}
