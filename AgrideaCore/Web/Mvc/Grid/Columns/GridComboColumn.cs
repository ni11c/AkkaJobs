using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridComboColumn<T, TValue> : GridEditColumn<T, TValue>
    {
        private readonly Func<T, IEnumerable<SelectListItem>> func_;
        private IEnumerable<SelectListItem> items_;

        public GridComboColumn(
            IGridModel<T> gridModel,
            string name,
            Func<T, TValue> func,
            Func<T, IEnumerable<SelectListItem>> funcData)
            : base(gridModel, name, func)
        {
            func_ = funcData;
        }

        public GridComboColumn(GridModel<T> gridModel,
            string name,
            Func<T, TValue> func,
            IEnumerable<SelectListItem> items)
            : base(gridModel, name, func)
        {
            items_ = items;
        }

        public override IHtmlString RenderInput(T dataItem)
        {
            TValue value = Value(dataItem);

            var items = items_ ?? func_(dataItem);
            if (items != null)

                items = items.SetSelectedValue(value.ToString());
            


            var modelName = Name; //ExpressionHelper.GetExpressionText(Expression);
            return GridModel.Helper.DropDownList(GridModel.BindingListName + "[" + GetIndex(dataItem) + "]." + modelName, items, GetAttributes(dataItem));
        }
    }
}