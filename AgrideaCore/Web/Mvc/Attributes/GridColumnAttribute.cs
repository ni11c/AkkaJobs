using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agridea.Web.Mvc
{
    public class GridColumnAttribute : Attribute
    {
        public GridColumnAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
