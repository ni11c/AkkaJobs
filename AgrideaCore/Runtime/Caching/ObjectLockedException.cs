
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization;

namespace Agridea.Runtime.Caching
{
    [Serializable]
    public class ObjectLockedException : Exception
    {
        #region Initialization
        public ObjectLockedException() : base() { }
        public ObjectLockedException(string message) : base(message) { }
        public ObjectLockedException(string message, Exception innerException) : base(message, innerException) { }
        protected ObjectLockedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion
    }
}
