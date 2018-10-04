namespace Agridea.Timers
{
  public interface ITime
  {
    /// <summary>
    /// Get the current time in ticks
    /// </summary>
    long CurrentTime{get;set;}

    /// <summary>
    /// Get the time resolution in ticks per second
    /// </summary>
    long TicksPerSecond{get;}

    /// <summary>
    /// Converts a tick count into seconds
    /// </summary>
    /// <param name="ticks">the tick count</param>
    /// <returns>the seconds</returns>
    double ConvertToSeconds(long ticks);

    /// <summary>
    /// Sleep for a delay in milliseconds
    /// </summary>
    /// <param name="delayInMilliseconds">delay in ms</param>
    void Sleep(int delayInMilliseconds);
  }
}
