using System;

namespace Agridea.Timers
{
    public interface ITimer : IDisposable
    {
        int Interval { get; }
        void Start(int intervalInMilliseconds);
        void Stop();

        event TimeEventHandler Tick;
    }
}
