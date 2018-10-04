
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization;

namespace Agridea.Runtime.Caching
{
    public class LockableValue
    {
        public LockableValue(object value, bool locked)
        {
            Value = value;
            Locked = locked;
        }
        public object Value { get; set; }
        public bool Locked { get; set; }
    }
}
