namespace Agridea.Runtime
{
    /// <summary>
    /// set properties are for virtual resource testing pattern
    /// </summary>
    public interface IMemory
    {
        long CurrentSizeInBytes { get; set; }
        double CurrentSizeInKiloBytes { get; set; }
        double CurrentSizeInMegaBytes { get; set; }
    }
}
