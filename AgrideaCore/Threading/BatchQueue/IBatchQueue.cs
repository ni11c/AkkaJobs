using System;
using System.Globalization;

namespace Agridea.Threading
{
    public interface IBatchQueue : IDisposable
    {
        CultureInfo CurrentUICulture { get; set; }
        IJob[] Jobs { get; }
        void Start();
        void Clear();
        void Add(IJob job);
        void Remove(IJob job);
        void Cancel(IJob job);
        void CancelPendingJobs();
        void CancelAll();
    }
}
