﻿using Agridea.Diagnostics.Contracts;
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
    public class WarningResult : ICounted
    {
        public string Counter { get; set; }
        public string Info { get; set; }
    }
}
