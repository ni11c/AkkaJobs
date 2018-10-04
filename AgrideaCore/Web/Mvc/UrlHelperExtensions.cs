using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Agridea.Diagnostics.Contracts;

namespace Agridea.Web.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string RouteUrl<TController>(this UrlHelper helper, Expression<Action<TController>> action) where TController : Controller
        {
            Requires<ArgumentNullException>.IsNotNull(action);
            return helper.RouteUrl(action, null);
        }
        public static string RouteUrl<TController>(this UrlHelper urlHelper, Expression<Action<TController>> action, object routeValues) where TController : Controller
        {
            Requires<ArgumentNullException>.IsNotNull(action);
            RouteValueDictionary newRouteValues = MvcExpressionHelper.GetRouteValuesFromExpression(action)
                .Merge(new RouteValueDictionary(routeValues));
            return urlHelper.RouteUrl(newRouteValues);
        }

        public static MvcHtmlString Javascript(this UrlHelper urlHelper, string fileName)
        {
            var source = urlHelper.Content(string.Format("~/Scripts/{0}", fileName));
            var script = string.Format(" <script src=\"{0}\" type=\"text/javascript\"></script>", source);
            return MvcHtmlString.Create(script);
        }
        public static MvcHtmlString StyleSheet(this UrlHelper urlHelper, string fileName, string media = "all")
        {
            var source = urlHelper.Content(string.Format("~/Content/{0}", fileName));
            var link = string.Format("<link href=\"{0}\" rel=\"Stylesheet\" type=\"text/css\" media=\"{1}\" />", source, media);
            return MvcHtmlString.Create(link);
        }

        public static MvcHtmlString Action<TController>(this UrlHelper urlHelper, Expression<Action<TController>> action) where TController : Controller
        {
            Requires<ArgumentNullException>.IsNotNull(action);
            var newRouteValues = MvcExpressionHelper.GetRouteValuesFromExpression(action);
            var result = MvcHtmlString.Create(urlHelper.RouteUrl(newRouteValues));
            return result;
        }

        #region DashBoard
        public static IHtmlString DashBoardFor<TController>(this UrlHelper urlHelper, Expression<Action<TController>> action, string title, string id, bool condition = true, bool hasData = false, bool isInvalid = false) where TController : Controller
        {
            if (!condition && !hasData)
                return MvcHtmlString.Create(string.Empty);

            var mainDiv = Tag.Div.Class("chrome");
            
            var titleDiv = Tag.Div.Class("title")
                .AddInnerHtml(title)
                .AddInnerHtml(Tag.Div.Class("expand-collapse")
                    .Html(Tag.A("#")
                        .Html(Tag.Img(urlHelper.Content("~/Content/Images/collapse_12x16.png"), "réduire"))));
            if (!condition || isInvalid)
                titleDiv.Style("background-color:red;");
            var contentDiv = Tag.Div.Class("content")
                .MergeAttribute("data-url", urlHelper.RouteUrl(action, null))
                .Id(id);
            return mainDiv.Html(titleDiv, contentDiv).ToMvcHtmlString();
        }
        #endregion

    }
}
