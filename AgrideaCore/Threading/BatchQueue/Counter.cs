using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;
using Agridea.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agridea.Threading
{
    public class Counter
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public int MaxCount { get; set; }
    }
}
