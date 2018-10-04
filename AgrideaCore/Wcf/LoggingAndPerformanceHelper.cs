using Agridea.Diagnostics.Logging;
using Agridea.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Agridea.Wcf
{
    public class LoggingAndPerformanceHelper
    {
        #region Constants
        #endregion

        #region Members
        private static Dictionary<OperationInfo, OperationPerformance> performances_ = new Dictionary<OperationInfo, OperationPerformance>();
        #endregion

        #region Initialization
        public void Reset()
        {
            performances_.Clear();
        }
        static LoggingAndPerformanceHelper()
        {
            LoggingSeverity = TraceEventType.Information;
            PerformanceMeasureOn = false;
        }
        #endregion

        #region Services
        public static TraceEventType LoggingSeverity { get; set; }
        public static bool PerformanceMeasureOn { get; set; }
        public static void StartOperation(string operation, string format, params object[] args)
        {
            Log.Write(LoggingSeverity, format, args);
            if (!PerformanceMeasureOn) return;
            Start(operation);
        }
        public static void EndOperation(string operation)
        {
            if (!PerformanceMeasureOn) return;
            End(operation);
        }
        public Dictionary<OperationInfo, OperationPerformance> Performances
        {
            get { return performances_; }
        }
        #endregion

        #region Helpers
        protected static void Start(string operation)
        {
            OperationPerformance value = GetPerformance(operation);
            value.Restart();
        }
        protected static void End(string operation)
        {
            OperationPerformance value = GetPerformance(operation);
            value.AddMeasure();
            Log.Info("{0} count {1}", operation, value.Count);
            Log.Info("-- TIME(ms)  last {0}, prev {1}, trend {2:0.00}, min {3}, max {4}, 50% {5}, 90% {6}", value.CpuLast.ToThousandsSeparated(), value.CpuPrevious.ToThousandsSeparated(), value.CpuTrend, value.CpuMin.ToThousandsSeparated(), value.CpuMax.ToThousandsSeparated(), value.CpuPercentile(50).ToThousandsSeparated(), value.CpuPercentile(90).ToThousandsSeparated());
            Log.Info("-- GC(bytes) last {0}, prev {1}, trend {2:0.00}, min {3}, max {4}, 50% {5}, 90% {6}", value.GcLast.ToThousandsSeparated(), value.GcPrevious.ToThousandsSeparated(), value.GcTrend, value.GcMin.ToThousandsSeparated(), value.GcMax.ToThousandsSeparated(), value.GcPercentile(50).ToThousandsSeparated(), value.GcPercentile(90).ToThousandsSeparated());
            Log.Info("-- WS(bytes) last {0}, prev {1}, trend {2:0.00}, min {3}, max {4}, 50% {5}, 90% {6}", value.WsLast.ToThousandsSeparated(), value.WsPrevious.ToThousandsSeparated(), value.WsTrend, value.WsMin.ToThousandsSeparated(), value.WsMax.ToThousandsSeparated(), value.WsPercentile(50).ToThousandsSeparated(), value.WsPercentile(90).ToThousandsSeparated());
        }
        protected static OperationPerformance GetPerformance(string operation)
        {
            OperationInfo key = new OperationInfo { OperationName = operation };
            if (!performances_.ContainsKey(key))
                performances_.Add(key, new OperationPerformance());
            return performances_[key];
        }
        #endregion
    }
    public struct OperationInfo
    {
        public string OperationName { get; set; }
    }
    public class OperationPerformance
    {
        private Stopwatch watch_ = Stopwatch.StartNew();
        private WorkingSet workingSet_ = new WorkingSet();
        private GarbageCollector garbageCollector_ = new GarbageCollector();
        private List<long> cpuMeasures_ = new List<long>();
        private List<long> gcMeasures_ = new List<long>();
        private List<long> wsMeasures_ = new List<long>();

        public void AddMeasure()
        {
            CpuPrevious = CpuLast;
            CpuLast = watch_.ElapsedMilliseconds;
            cpuMeasures_.Add(CpuLast);
            cpuMeasures_.Sort();

            GcPrevious = GcLast;
            GcLast = garbageCollector_.UsedBytes;
            gcMeasures_.Add(GcLast);
            gcMeasures_.Sort();

            WsPrevious = WsLast;
            WsLast = workingSet_.UsedBytes;
            wsMeasures_.Add(WsLast);
            wsMeasures_.Sort();
        }

        public void Restart()
        {
            watch_.Restart();
        }

        public int Count { get { return cpuMeasures_.Count; } }
        public long CpuPrevious { get; set; }
        public long CpuLast { get; set; }
        public long CpuMin { get { return cpuMeasures_.FirstOrDefault(); } }
        public long CpuMax { get { return cpuMeasures_.LastOrDefault(); } }
        public long CpuPercentile(int nth)
        {
            if (cpuMeasures_.Count <= 0) return 0;
            var index = nth * cpuMeasures_.Count / 100;
            return cpuMeasures_[index];
        }
        public double CpuTrend
        {
            get { return CpuLast.PercentIncrease(CpuPrevious); }
        }

        public long GcPrevious { get; set; }
        public long GcLast { get; set; }
        public long GcMin { get { return gcMeasures_.FirstOrDefault(); } }
        public long GcMax { get { return gcMeasures_.LastOrDefault(); } }
        public long GcPercentile(int nth)
        {
            if (gcMeasures_.Count <= 0) return 0;
            var index = nth * gcMeasures_.Count / 100;
            return gcMeasures_[index];
        }
        public double GcTrend
        {
            get { return GcLast.PercentIncrease(GcPrevious); }
        }

        public long WsPrevious { get; set; }
        public long WsLast { get; set; }
        public long WsMin { get { return wsMeasures_.FirstOrDefault(); } }
        public long WsMax { get { return wsMeasures_.LastOrDefault(); } }
        public long WsPercentile(int nth)
        {
            if (wsMeasures_.Count <= 0) return 0;
            var index = nth * wsMeasures_.Count / 100;
            return wsMeasures_[index];
        }
        public double WsTrend
        {
            get { return WsLast.PercentIncrease(WsPrevious); }
        }
    }
}

