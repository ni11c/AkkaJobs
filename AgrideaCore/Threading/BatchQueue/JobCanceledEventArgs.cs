using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agridea.Threading
{
    [Serializable]
    public class JobCanceledEventArgs : EventArgs
    {
        public JobCanceledEventArgs()
        {
        }
    }
}
