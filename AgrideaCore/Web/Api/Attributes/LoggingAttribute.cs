using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;
using Agridea.Runtime;
using Agridea.Web.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Agridea.Web.Api.ActionFilters
{
    public class LoggingAttribute : ActionFilterAttribute
    {
        #region Constants
        private static readonly string ControllerNameSuffix = "Controller";
        #endregion

        #region Members
        [WebFarmCompatibleAttribute(Compatible = false, Reason = "Does not cumulate actions performances across webfarm elements but inside one particular")]
        private static ConcurrentDictionary<ActionInfo, ActionPerformance> performances_ = new ConcurrentDictionary<ActionInfo, ActionPerformance>();
        #endregion

        #region Named Parameters
        public static TraceEventType LoggingSeverity { get; set; }
        public static bool PerformanceMeasureOn { get; set; }
        #endregion

        #region Initialization
        public void Reset()
        {
            performances_.Clear();
        }
        public LoggingAttribute()
        {
            //LoggingSeverity = TraceEventType.Information;
            //PerformanceMeasureOn = false;
        }
        #endregion

        #region Services
        public ConcurrentDictionary<ActionInfo, ActionPerformance> Performances
        {
            get { return performances_; }
        }
        #endregion

        #region ActionFilterAttribute
        Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            return null;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            Start(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionContext)
        {
            base.OnActionExecuted(actionContext);
            End(actionContext);
        }
        #endregion

        #region Helpers
        protected virtual string GetMessage(HttpActionContext actionContext)
        {
            return string.Format("User='{0}' '{1}/{2}({3})' Parameters='{4}'",
                HttpActionContextHelper.GetSessionId(actionContext),
                HttpActionContextHelper.GetControllerName(actionContext),
                HttpActionContextHelper.GetActionName(actionContext),
                HttpActionContextHelper.GetHttpMethod(actionContext),
                HttpActionContextHelper.GetParameters(actionContext));
        }
        private Type GetControllerType(HttpActionContext actionContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(actionContext.ActionDescriptor);
            Asserts<ArgumentNullException>.IsNotNull(actionContext.ActionDescriptor.ControllerDescriptor);
            return HttpActionContextHelper.GetController(actionContext);
        }
        private string GetActionName(HttpActionExecutedContext actionContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(actionContext.ActionContext);
            return string.Format("{0}({1})", actionContext.ActionContext.ActionDescriptor.ActionName, actionContext.ActionContext.Request.Method);
        }
        private Type GetControllerType(HttpActionExecutedContext actionContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(actionContext.ActionContext);
            Asserts<ArgumentNullException>.IsNotNull(actionContext.ActionContext.ControllerContext.ControllerDescriptor);
            return actionContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType;
        }
        private string GetActionName(HttpActionContext actionContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(actionContext.ActionDescriptor);
            return string.Format("{0}({1})", actionContext.ActionDescriptor.ActionName, actionContext.Request.Method);
        }
        protected void Start(HttpActionContext actionContext)
        {
            Log.Write(LoggingSeverity, GetMessage(actionContext));
            if (!PerformanceMeasureOn) return;
            var value = GetPerformance(GetControllerType(actionContext), GetActionName(actionContext));
            if (value == null) return;
            value.Restart();
        }
        protected void End(HttpActionExecutedContext actionContext)
        {
            if (!PerformanceMeasureOn) return;
            var controllerType = GetControllerType(actionContext);
            var actionName = GetActionName(actionContext);
            var value = GetPerformance(controllerType, actionName);
            if (value == null) return;
            value.AddMeasure();
            var message =
                string.Format("{0}/{1} count {2}\n", controllerType.Name.Replace(ControllerNameSuffix, string.Empty), actionName, value.Count) +
                string.Format("-- TIME(ms)  last {0}, prev {1}, trend {2:0.00}, min {3}, max {4}, 50% {5}, 90% {6}\n", value.CpuLast.ToThousandsSeparated(), value.CpuPrevious.ToThousandsSeparated(), value.CpuTrend, value.CpuMin.ToThousandsSeparated(), value.CpuMax.ToThousandsSeparated(), value.CpuPercentile(50).ToThousandsSeparated(), value.CpuPercentile(90).ToThousandsSeparated()) +
                string.Format("-- GC(bytes) last {0}, prev {1}, trend {2:0.00}, min {3}, max {4}, 50% {5}, 90% {6}\n", value.GcLast.ToThousandsSeparated(), value.GcPrevious.ToThousandsSeparated(), value.GcTrend, value.GcMin.ToThousandsSeparated(), value.GcMax.ToThousandsSeparated(), value.GcPercentile(50).ToThousandsSeparated(), value.GcPercentile(90).ToThousandsSeparated()) +
                string.Format("-- WS(bytes) last {0}, prev {1}, trend {2:0.00}, min {3}, max {4}, 50% {5}, 90% {6}", value.WsLast.ToThousandsSeparated(), value.WsPrevious.ToThousandsSeparated(), value.WsTrend, value.WsMin.ToThousandsSeparated(), value.WsMax.ToThousandsSeparated(), value.WsPercentile(50).ToThousandsSeparated(), value.WsPercentile(90).ToThousandsSeparated());
            Log.Info(message);
        }
        protected ActionPerformance GetPerformance(Type controllerType, string actionName)
        {
            if (controllerType == null || actionName == null) return null;
            ActionInfo key = new ActionInfo { ControllerType = controllerType, ActionName = actionName };

            return performances_.GetOrAdd(key, k => new ActionPerformance());
        }
        #endregion
    }
    public struct ActionInfo
    {
        public Type ControllerType { get; set; }
        public string ActionName { get; set; }
    }
    public class ActionPerformance
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
