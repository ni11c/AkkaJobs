using System.Collections.Generic;

namespace Agridea.Web.Mvc.Grid
{
    public interface IOrdering
    {
        IList<SortOption> Orders { get; }
    }
}
