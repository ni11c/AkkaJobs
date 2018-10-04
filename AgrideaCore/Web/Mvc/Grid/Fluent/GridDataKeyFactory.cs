using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public class GridDataKeyFactory<T>
    {
        public IList<IGridDataKey<T>> DataKeys { get; private set; }
        public GridDataKeyFactory(IList<IGridDataKey<T>> dataKeys)
        {
            DataKeys = dataKeys;
        }

        public IGridDataKey<T> Add<TValue>(string key, Func<T, TValue> func)
        {
            var dataKey = new GridDataKey<T, TValue>(key, func);
            DataKeys.Add(dataKey);
            return dataKey;
        }
    }
}
