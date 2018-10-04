using System;
using System.Runtime.Serialization;

namespace Agridea.DataRepository
{
    [Serializable]
    public class IntegrityException : Exception
    {
        #region Initialization
        public IntegrityException() : base() { }
        public IntegrityException(string message) : base(message) { }
        public IntegrityException(string message, Exception innerException) : base(message, innerException) { }
        protected IntegrityException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion
    }
}
