using System.Security.AccessControl;
using Agridea.Diagnostics.Contracts;
using Agridea.Web.Mvc.Grid.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridBoundColumn<T, TValue> : GridColumnBase<T>, IGridBoundColumn<T>
    {
        #region Members

        private readonly ControllerContext viewContext_;
        private bool isSortable_;

        #endregion Members

        #region Initialization

        public GridBoundColumn(IGridModel<T> gridModel, string name, Func<T, TValue> func)
            : base(gridModel, name)
        {
            Value = func;
            viewContext_ = gridModel.Context;
            Orders = gridModel.Ordering.Orders;
            isSortable_ = true;
            PredicateCssClasses = new List<Func<T, string>>();
        }

        public IList<SortOption> Orders { get; private set; }

        public Func<T, bool> HideWhenFunc { get; set; }

        public Func<T, bool> DisabledFunc { get; set; }

        public object DefaultValue { get; set; }

        public Expression<Func<T, object>> SortExpression { get; set; }

        public Func<T, bool> WarningPredicate { get; set; }

        public string WarningText { get; set; }

        public Func<T, TValue> Value { get; private set; }

        public AggregateFunction AggregateFunction { get; set; }

        #endregion Initialization

        #region Services

        public bool IsSortable
        {
            get
            {
                return isSortable_ && CheckSortability();
            }
            set { isSortable_ = value; }
        }

        public List<Func<T, string>> PredicateCssClasses { get; set; }

        public string Format { get; set; }


        public override FluentTagBuilder GetContentTag(T dataItem)
        {
            var tag = base.GetContentTag(dataItem);
            if (DisabledFunc != null && DisabledFunc(dataItem))
                tag.Attributes.Add("style", "background:#EEE");
            PredicateCssClasses.ForEach(predicate => tag.AddCssClass(predicate(dataItem)));
            return tag;
        }
        public override string GetContent(T dataItem)
        {
            return GetFormattedValue(dataItem);
        }

        public override string GetExportContent(T dataItem)
        {
            return GetFormattedValue(dataItem);
        }

        public override string GetWarning(T dataItem)
        {
            if (WarningPredicate != null && WarningPredicate(dataItem))
                return Tag.Span.Class("grid-icon", GridClass.WarningIcon).Title(WarningText).ToString();

            return string.Empty;
        }

        public override string GetHeader()
        {
            var htmlFieldName = Name; //ExpressionHelper.GetExpressionText(Expression);
            var modelMetadata = ModelMetadata.FromStringExpression(typeof(T).Name, GridModel.ViewData);
            var resolvedLabelText = Title ?? modelMetadata.DisplayName ?? modelMetadata.PropertyName ?? htmlFieldName.Split('.').Last();
            var content = string.Empty;
            if (!IsSortable)
                content = resolvedLabelText ?? content;
            else
                content = String.IsNullOrEmpty(resolvedLabelText)
                       ? string.Empty
                       : BuildUrl(GetSortPropertyName(), resolvedLabelText);
            return content;
        }

        public override FluentTagBuilder RenderFooter()
        {
            if (AggregateFunction == null) return base.RenderFooter();
            var aggregateMethod = typeof(Enumerable)
                .GetMethod(AggregateFunction.FunctionName,
                           BindingFlags.Public | BindingFlags.Static,
                           null,
                           new[] { typeof(IEnumerable<TValue>) },
                           null);
            Asserts<InvalidOperationException>.IsNotNull(aggregateMethod, string.Format("Method {0} is not available for type {1}", AggregateFunction.FunctionName, typeof(TValue).Name));
            var content = (TValue)aggregateMethod.Invoke(null, new object[] { GridModel.PaginatedItems.Select(Value) });
            return base.RenderFooter().Html(FormatResult(content));
        }

        protected string GetFormattedValue(T dataItem)
        {
            if (HideWhenFunc != null && HideWhenFunc(dataItem))
                return string.Empty;

            var content = Value(dataItem);
            if (typeof(TValue) == typeof(bool) || typeof(TValue) == typeof(bool?))
                return Convert.ToBoolean(content) ? "Oui" : "Non";

            return FormatResult(content);
        }

        private string BuildUrl(string sortPropertyName, string linkText)
        {
            var orderDirectionQuery = string.Empty;
            var routeValues = new RouteValueDictionary();
            foreach (var key in viewContext_.RequestContext.HttpContext.Request.QueryString.AllKeys.Where(key => key != null))
            {
                routeValues[key] = viewContext_.RequestContext.HttpContext.Request.QueryString[key];
            }
            var arrowSpan = Tag.Span.Class("t-order-icon");
            if (Orders.Any(x => x.PropertyName == sortPropertyName))
            {
                if (Orders.First(x => x.PropertyName == sortPropertyName).Direction == GridParameters.Ascending)
                {
                    orderDirectionQuery += sortPropertyName + GridParameters.PropertyDirectionSeparator + GridParameters.Descending;
                    AddOrUpdate(routeValues, GridParameters.OrderKey, orderDirectionQuery);
                    arrowSpan.Class(GridClass.TopIcon);
                }
                else
                {
                    routeValues.Remove(GridParameters.OrderKey);
                    arrowSpan.Class(GridClass.BottomIcon);
                }
            }
            else
            {
                orderDirectionQuery += sortPropertyName + GridParameters.PropertyDirectionSeparator + GridParameters.Ascending;
                AddOrUpdate(routeValues, GridParameters.OrderKey, orderDirectionQuery);
            }
            var url = UrlHelper.GenerateUrl(null, null, null, routeValues, RouteTable.Routes, GridModel.Context.RequestContext, true);
            var link = Tag.A(url).Id(sortPropertyName).Class(GridClass.GridShift).Html(linkText).Html(arrowSpan.Style("display:inline-block")).ToString();
            return link;
        }

        protected static void AddOrUpdate(RouteValueDictionary routeValues, string key, object value)
        {
            if (routeValues.ContainsKey(key))
                routeValues[key] = value;
            else
                routeValues.Add(key, value);
        }

        protected bool CheckSortability()
        {
            var propertyName = GetSortPropertyName();
            return typeof(T).PropertyExists(propertyName) && typeof(T).IsWritable(propertyName);
        }

        protected string GetSortPropertyName()
        {
            return SortExpression == null
                ? Name //ExpressionHelper.GetExpressionText(Expression)
                : string.Join(".", RemoveUnary(SortExpression.Body).ToString().Split('.').Skip(1));
        }

        protected static Expression RemoveUnary(Expression body)
        {
            var unary = body as UnaryExpression;
            return unary != null ? unary.Operand : body;
        }

        private string FormatResult(TValue value)
        {
            if (value == null)
                return string.Empty;

            return !String.IsNullOrEmpty(Format)
                ? string.Format(Format, value)
                : value.ToString();
        }

        #endregion Services
    }
}