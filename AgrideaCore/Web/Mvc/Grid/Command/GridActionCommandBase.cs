using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Agridea.Web.Mvc.Grid.Command
{
    public abstract class GridActionCommandBase<T> : ICommand<T> 
    {
        #region Initialization
        protected GridActionCommandBase(ControllerContext context, string imageUrl, string alternateText, RouteValueDictionary routeValues, object htmlAttributes)
        {
            ImageUrl = imageUrl;
            AlternateText = alternateText;
            HtmlAttributes = htmlAttributes;
            Context = context;
            RouteValueDictionary = routeValues;
            MergeRouteValuesWithRequestContext();
        }
        #endregion

        #region Services
        public RouteValueDictionary RouteValueDictionary { get; protected set; }
        public string AlternateText { get;  set; }
        public string ImageUrl { get; protected set; }
        public object HtmlAttributes { get; private set; }
        public ControllerContext Context { get; private set; }
        public bool TextOnly { get; set; }
        public Func<T, bool> HideWhenFunc { get; set; }
        public bool IsHidden { get; set; }

        public virtual string Render(T dataItem, IList<IGridDataKey<T>> dataKeys, ControllerContext context)
        {
            if (CheckIfHidden(dataItem)) return string.Empty;
            var url = dataItem == null
                ? context.BuildUrl(RouteValueDictionary, dataKeys) 
                : context.BuildUrl(dataItem, RouteValueDictionary, dataKeys);
            if (!TextOnly)
            {
                var image = Tag.A(url).Style("display:inline-block")
                               .Html(Tag.Span.Class("grid-icon", ImageUrl).Title(AlternateText))
                               .MergeAttributes(HtmlAttributes.DictionaryParse());
                return image.ToString();
            }
            return Tag.A(url).Style("display:inline-block").Class(ImageUrl)
                      .Text(AlternateText).MergeAttributes(HtmlAttributes.DictionaryParse())
                      .ToString();
        }

        protected bool CheckIfHidden(T dataItem)
        {
            return IsHidden || (HideWhenFunc != null && HideWhenFunc(dataItem));
        }

        #endregion

        #region Helpers

        protected void MergeRouteValuesWithRequestContext()
        {
            var excludedKeys = new[] {GridParameters.ActionKey, GridParameters.ControllerKey};
            foreach (var routeDataValues in Context.RequestContext.RouteData.Values
                                                   .Where(m =>
                                                          !excludedKeys.Contains(m.Key) &&
                                                          RouteValueDictionary.ContainsKey(m.Key)))
                RouteValueDictionary[routeDataValues.Key] = routeDataValues.Value;

        }

        #endregion
    }
}
