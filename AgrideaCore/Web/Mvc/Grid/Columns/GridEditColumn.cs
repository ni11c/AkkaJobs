using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public abstract class GridEditColumn<T, TValue> : GridBoundColumn<T, TValue>, IGridEditColumn<T>
    {
        #region Initialization
        protected GridEditColumn(IGridModel<T> gridModel, string name, Func<T, TValue> func)
            : base(gridModel, name, func)
        {
            IsDisabled = false;
        }

        #endregion Initialization

        #region Properties

        public string Classes { get; set; }

        

        public Func<T, bool> ReadOnlyFunc { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsHidden { get; set; }

        public int? InputSize { get; set; }

        public int? StyleWidth { get; set; }
        public string PlaceHolder { get; set; }

        public abstract IHtmlString RenderInput(T dataItem);

        

        #endregion Properties

        #region Services

        

        public override string GetContent(T dataItem)
        {
            return (HideWhenFunc != null && HideWhenFunc(dataItem))
                       ? new GridHiddenColumn<T, TValue>(GridModel, Name, Value, DefaultValue).GetContent(dataItem)
                       : RenderInput(dataItem).ToString();
        }
        
        protected bool GetDisabled(T dataItem)
        {
            return DisabledFunc != null
                ? DisabledFunc(dataItem)
                : IsDisabled;
        }

        protected bool GetReadOnly(T dataItem)
        {
            return ReadOnlyFunc != null
                ? ReadOnlyFunc(dataItem)
                : IsReadOnly;
        }
        
        public int GetIndex(T dataItem)
        {
            return GridModel.PaginatedItems.IndexOf(dataItem);
        }

        public virtual IDictionary<string, object> GetAttributes(T dataItem)
        {
            var dic = new Dictionary<string, object>();
            if (GetDisabled(dataItem))
            {
                dic.Add("disabled", "disabled");
            }
            if (GetReadOnly(dataItem))
            {
                dic.Add("readonly", "readonly");
            }
            if (InputSize.HasValue)
                dic.Add("size", InputSize);

            if (StyleWidth.HasValue)
                dic.Add("style", "max-width:" + StyleWidth.Value + "px");

            if (!string.IsNullOrWhiteSpace(PlaceHolder))
                dic.Add("placeholder", PlaceHolder);

            return dic;
        }

        public string GetIndexedName(T dataItem)
        {
            var propertyName = Name; //ExpressionHelper.GetExpressionText(Expression);
            return string.Format("{0}[{1}].{2}", GridModel.BindingListName, GetIndex(dataItem), propertyName);
        }

        #endregion Services
    }
}
