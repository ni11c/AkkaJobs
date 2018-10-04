using System.Diagnostics;
namespace Agridea.Runtime
{
    public static class PerformanceHelper
    {
        public static float GetCurrentCpuPercentage()
        {
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            CounterSample cpuCounterSample1 = cpuCounter.NextSample();
            System.Threading.Thread.Sleep(500);
            CounterSample cpuCounterSample2 = cpuCounter.NextSample();
            return CounterSample.Calculate(cpuCounterSample1, cpuCounterSample2);
        }
        public static float GetCurrentMemoryPercentage()
        {
            return new PerformanceCounter("Memory", "% Committed Bytes in Use").NextValue();
        }
        public static float GetCurrentWaitingRequests()
        {
            return new PerformanceCounter("ASP.NET", "Requests Queued").NextValue();
        }
    }
}
