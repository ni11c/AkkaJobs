using System;
using Agridea.Diagnostics.Logging;

namespace Agridea
{
    /// <summary>
    /// Guidelines : dont use this class if a finalizer is not necessary
    /// because this is costly. Instead use Disposable from which this class
    /// inherits from.
    /// </summary>
    [Serializable]
    public abstract class Finalizable : Disposable
    {
        #region Finalization
        ~Finalizable()
        {
            Log.Info("Agridea.Diagnostics.Disposable.Dispose().~Finalizable().Entry");
            Dispose(false);
            Log.Info("Agridea.Diagnostics.Disposable.Dispose().~Finalizable().Exit");
        }
        #endregion
    }
}
