
namespace Agridea
{
    public interface IRandomGenerator
    {
        void Seed(int seed);
        void SeedWithClock();
        int Next();
        int Next(int inf, int sup);
        bool Next(int prob);
    }
}
