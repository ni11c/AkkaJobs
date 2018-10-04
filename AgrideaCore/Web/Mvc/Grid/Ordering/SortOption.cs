using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agridea.Web.Mvc.Grid
{
    public class SortOption 
    {
        public SortOption(string propertyName, string direction = GridParameters.Ascending, bool isDynamic = false)
        {
            PropertyName = propertyName;
            IsDynamic = isDynamic;
            Direction = direction;
        }
        public string PropertyName { get; set; }
        public string Direction { get; set; }
        public bool IsDynamic { get; set; }
    }
}
