using Agridea.DataRepository;
using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;
using Agridea.Threading;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Agridea.Runtime;

namespace Agridea.Threading
{
    public interface IBatchQueueJob : IJob
    {
        void OnCounter(object sender, Counter counter);
        void OnError(object sender, ErrorResult result);
        void OnWarning(object sender, WarningResult result);
        void OnFinished(object sender, FinishedResult result);
    }
}
