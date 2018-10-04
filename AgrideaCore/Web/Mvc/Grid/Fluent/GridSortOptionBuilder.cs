using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public class GridSortOptionBuilder
    {
        
        public GridSortOptionBuilder(SortOption sortOption)
        {
            SortOption = sortOption;
        }

        protected SortOption SortOption { get; private set; }

        public virtual void Descending()
        {
            SortOption.Direction = GridParameters.Descending;
        }
        public virtual void Ascending()
        {
            SortOption.Direction = GridParameters.Ascending;
        }
    }
}
