using System;
using System.Collections.Generic;
using System.Linq;

namespace Agridea.Web.Mvc.Grid
{
    public class Ordering : IOrdering
    {
        #region Members
        private readonly string orderQuery_;
        private readonly Type gridType_;
        #endregion

        #region Initialization
        public Ordering(string orderQuery, Type gridType)
        {
            orderQuery_ = orderQuery;
            gridType_ = gridType;
            Orders = new List<SortOption>();
            ParseOrderQuery();
        }
        #endregion

        #region Properties
        public IList<SortOption> Orders { get; private set; }
        #endregion

        #region Helpers
        /// <summary>
        /// Parse the query string (Order key) : value must be of type : FirstPropertyName~FirstDirection-SecondPropertyName~SecondDirection
        /// Check if properties belong to model (don't trust client input).
        /// </summary>
        private void ParseOrderQuery()
        {
            if (string.IsNullOrEmpty(orderQuery_)) return;

            var possibleDirection = new[] { GridParameters.Descending, GridParameters.Ascending };
            foreach (var order in orderQuery_.Split(GridParameters.OrderSeparator).Where(m => m.Contains(GridParameters.PropertyDirectionSeparator)))
            {
                var propertyDirection = order.Split(GridParameters.PropertyDirectionSeparator.ToCharArray());
                var propertyName = propertyDirection.First();
                var direction = propertyDirection.Last();
                if (!gridType_.PropertyExists(propertyName) || !gridType_.IsWritable(propertyName)) continue;
                if (!possibleDirection.Contains(direction)) continue;
                Orders.Add(new SortOption(propertyName, direction, true));
            }
        }
        public override string ToString()
        {
            return string.Join(GridParameters.OrderSeparator.ToString(), Orders.Select(m => m.PropertyName + GridParameters.PropertyDirectionSeparator + m.Direction));
        }
        #endregion
    }
}
