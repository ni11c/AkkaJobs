
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization;

namespace Agridea.Runtime
{
    [Serializable]
    public class WarningException : Exception
    {
        #region Initialization
        public WarningException() : base() { }
        public WarningException(string message) : base(message) { }
        public WarningException(string message, Exception innerException) : base(message, innerException) { }
        protected WarningException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion
    }
}
