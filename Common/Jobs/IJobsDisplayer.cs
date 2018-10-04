namespace Agridea.Prototypes.Akka.Common
{
    public interface IJobsDisplayer<in T> 
    {
        void Display(T jobs);
    }
}
