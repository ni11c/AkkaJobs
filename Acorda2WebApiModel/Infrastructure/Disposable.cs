using System;
using System.Threading;

namespace Agridea.Acorda.Acorda2.WebApi.Model
{
    [Serializable]
    public abstract class Disposable : IDisposable
    {
        #region Members
        private int disposed_;
        #endregion

        #region IDisposable
        public void Dispose()
        {
            //Log.Verbose(string.Format("'{0}:{1}' Dispose()", GetType().Name, GetHashCode()));
            if (Interlocked.CompareExchange(ref disposed_, 1, 0) == 1) return;
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected abstract void Dispose(bool disposing);
        #endregion

        #region Queries
        public bool Disposed { get { return disposed_ != 0; } }
        #endregion
    }
}
