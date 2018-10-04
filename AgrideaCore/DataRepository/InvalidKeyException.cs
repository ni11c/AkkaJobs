using System;
using System.Runtime.Serialization;

namespace Agridea.DataRepository
{
    [Serializable]
    public class InvalidKeyException : Exception
    {
        #region Initialization
        public InvalidKeyException() : base() { }
        public InvalidKeyException(string message) : base(message) { }
        public InvalidKeyException(string message, Exception innerException) : base(message, innerException) { }
        protected InvalidKeyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion
    }
}
