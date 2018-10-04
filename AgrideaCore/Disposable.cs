using System;
using System.Threading;

namespace Agridea
{
    /// <summary>
    /// Base class for root class needing to implement the Dispose pattern
    /// Implement as follows in subclasses.
    /// Remarks : 
    /// - try/finally to be removed for first level heirs 
    /// - namely those inheriting directly from Disposable
    /// protected override void Dispose (bool disposing)
    /// {
    ///    try
    ///    {
    ///      if(disposing)
    ///      {
    ///        //Release managed resources.
    ///      }
    ///
    ///      //Release unmanaged resources.
    ///    }
    ///    finally
    ///    {
    ///       base.Dispose(disposing);
    ///    }
    /// }    
    /// </summary>
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
        }
        protected abstract void Dispose(bool disposing);
        #endregion

        #region Queries
        public bool Disposed { get { return disposed_ != 0; } }
        #endregion
    }
}
