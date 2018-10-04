using System;

namespace Agridea.Threading
{
    public interface IJob : IEquatable<IJob>, ICancelable
    {
        string Code { get; set; }
        DateTime RequestDate { get; set; }
        string Input { get; set; }
        string RequestedBy { get; set; }
        string[] Parameters { get; set; }
        string ProgressRate { get; set; }
        JobStates JobState { get; set; }
        string TimeElapsed { get; set; }
        int WarningCount { get; set; }
        int ErrorCount { get; set; }
        string Result { get; set; }
        string Diagnostics { get; set; }

        void Reset();

        void Add();
        event EventHandler Added;

        void Start();
        event EventHandler Started;

        void Run();
        event OperationProgressEventHandler Progress;

        void Finish();
        event EventHandler Finished;

        void Cancel();
        event JobCanceledEventHandler Canceled;

        void Fail(Exception e);
        event JobFailedEventHandler Failed;
    }
}
