using System;
using System.Linq;
using System.Linq.Expressions;
using Agridea.Diagnostics.Contracts;

namespace Agridea.Web.Mvc.Grid
{
    public class GridDataKey<T, TValue> : IGridDataKey<T>
    {
        #region Initialization
        public GridDataKey(string key, Func<T, TValue> func)
        {
            Name = key;
            Value = func;
        }

        #endregion

        #region Services
        public string Name { get; set; }
        public Func<T, TValue> Value { get; set; }
        public object GetValue(T dataItem)
        {
            return Value(dataItem);
        }
        #endregion
    }
}
