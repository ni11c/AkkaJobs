using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Agridea.Diagnostics.Contracts;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public class GridSortFactory<T> :IHideObjectMembers
    {
        
        public GridSortFactory(IOrdering ordering)
        {
            Ordering = ordering;
        }

        protected IOrdering Ordering { get; private set; }

        public GridSortOptionBuilder Add<TValue>(Expression<Func<T, TValue>> expression)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            Asserts<ArgumentException>.IsTrue(typeof (T).IsWritable(propertyName), "Property {0} cannot be used as a sorting property");
            return Add(new SortOption(propertyName));
        }

        private GridSortOptionBuilder Add(SortOption sortOption)
        {
            //don't add any default order if there are orderings from queryString
            if (!Ordering.Orders.Any(x => x.IsDynamic))
                Ordering.Orders.Add(sortOption);
            return new GridSortOptionBuilder(sortOption);
        }
    }
}
