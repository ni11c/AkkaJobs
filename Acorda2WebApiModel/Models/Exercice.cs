
namespace Agridea.Acorda.Acorda2.WebApi.Model
{
    public partial class Exercice
    {
        public int Year { get; set; }

        public override string ToString()
        {
            return string.Format("[Exercice Year={0}]", Year);
        }
    }
}
