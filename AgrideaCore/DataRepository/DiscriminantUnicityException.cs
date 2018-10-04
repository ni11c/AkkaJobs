using System;
using System.Runtime.Serialization;

namespace Agridea.DataRepository
{
    [Serializable]
    public class DiscriminantUnicityException : Exception
    {
        #region Initialization
        public DiscriminantUnicityException() : base() { }
        public DiscriminantUnicityException(string message) : base(message) { }
        public DiscriminantUnicityException(string message, Exception innerException) : base(message, innerException) { }
        protected DiscriminantUnicityException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion
    }
}
