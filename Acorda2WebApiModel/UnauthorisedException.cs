using System;
using System.Runtime.Serialization;

namespace Agridea.Acorda.Acorda2.WebApi.Model
{
    [Serializable]
    public class UnauthorizedException : Exception
    {
        #region Initialization
        public UnauthorizedException() : base() { }
        public UnauthorizedException(string message) : base(message) { }
        public UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
        protected UnauthorizedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion
    }
}
