using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agridea.Threading
{
    [Serializable]
    public class JobFailedEventArgs : EventArgs
    {
        public JobFailedEventArgs(Exception e)
        {
            Exception = e;
        }
        public Exception Exception { get; private set; }
    }
}
