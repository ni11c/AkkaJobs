using System;
using System.Runtime.Serialization;

namespace Agridea.Prototypes.Akka.Common
{
    /// <summary>
    /// Does it make sense to define interfaces for progress, status and result, if they are
    /// not used outside of IJob ? Probably not.
    /// Also, does it make sense to define other kinds of job interfaces (like IPausableJob, ICancelableJob) ?
    /// Problem is, JobActor relies on one IJob interface. Would need to define other kinds of job actors too
    /// (PausableJobActor, CancelableJobActor). Is it worth the hassle ? Why not simply dummy-implement
    /// the unused behaviour ? Well it is not semantically correct...
    /// </summary>
    public interface IJob : ICanReportProgress, ICanReportStatus, ICanGetResult, ICancelable, IPausable
    {
        string Name { get; }
        JobStatus Status { get; }
        void Run();
    }

    public interface ICanReportStatus
    {
        event Action<JobStatus> StatusChanged;
    }

    public interface ICanReportProgress
    {
        event Action<int> Progressed;
        event Action Finished;
        event Action Started;
    }

    public interface ICancelable
    {
        void Cancel();
    }

    public interface IPausable
    {
        void Pause();
        void Resume();
    }

    /// <summary>
    /// Enforce string as the result type. May look restrictive but results shall not be used to pass extensive amounts
    /// of data. Instead, output data may be written in some persistence medium by the job, and the result shall only
    /// contain info on how to access the output data.
    /// </summary>
    public interface ICanGetResult
    {
        string GetResult();
    }
}