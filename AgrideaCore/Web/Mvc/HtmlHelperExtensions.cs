using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Agridea.Diagnostics.Contracts;
using Agridea.Resources;

namespace Agridea.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        #region ActionLinkSpan

        public static string ActionLinkSpan(this HtmlHelper helper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, string> htmlAttributes)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var url = urlHelper.Action(actionName, routeValues);
            var link = !string.IsNullOrEmpty(linkText) ? HttpUtility.HtmlEncode(linkText) : string.Empty;

            //var innerTagBuilder = Tag.Span
            //    .Html(!String.IsNullOrEmpty(linkText) ? HttpUtility.HtmlEncode(linkText) : string.Empty);

            //FluentTagBuilder tagBuilder = new FluentTagBuilder(FluentTagBuilder.A)
            //    .AddInnerHtml(innerTagBuilder.ToString(TagRenderMode.Normal))
            //    .MergeAttributes(htmlAttributes)
            //    .MergeAttribute(FluentTagBuilder.Href, url);

            //return tagBuilder.ToString(TagRenderMode.Normal);
            return Tag.A(url)
                      .Html(Tag.Span.Html(link))
                      .MergeAttributes(htmlAttributes).ToString();
        }

        #endregion

        #region SubmitButton

        public static MvcHtmlString SubmitButton(this HtmlHelper helper, string name, string id, string value, string imagePath = null, string cssClass = null)
        {
            var inputBuilder = Tag.Input(SubmitButtonType, value);
            if (!string.IsNullOrEmpty(name))
                inputBuilder.Name(name);
            if (!string.IsNullOrEmpty(id))
                inputBuilder.Id(id);
            if (!string.IsNullOrEmpty(cssClass))
                inputBuilder.AddCssClass(cssClass);
            if (!string.IsNullOrWhiteSpace(imagePath))
                inputBuilder.Html(Tag.Img(imagePath));
            return inputBuilder.ToMvcHtmlString(TagRenderMode.SelfClosing);
        }

        #endregion

        #region SearchButton

        public static MvcHtmlString SearchButton(this HtmlHelper helper, string id = null, string name = null)
        {
            return helper.SubmitButton(name, id, AgrideaCoreStrings.WebSearchButtonCaption);
        }

        #endregion

        #region SendPasswordButton

        public static MvcHtmlString SendPasswordByEmailButton(this HtmlHelper helper, string id = null)
        {
            return helper.SubmitButton("send", id, AgrideaCoreStrings.WebSendNewPasswordByEmailCaption);
        }

        #endregion

        #region MultilineText

        public static MvcHtmlString DisplayMultilineTextFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var text = html.DisplayTextFor(expression).ToString();
            return string.IsNullOrEmpty(text)
                       ? MvcHtmlString.Empty
                       : MvcHtmlString.Create(text.Replace(Environment.NewLine, Tag.Br));
        }

        #endregion

        #region RadioButtons

        public static IHtmlString RadioButtonYesNoFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool displayOnTwoLines = true)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Format("<span style='white-space:nowrap;'>{0}{1}</span>", html.RadioButtonFor(expression, true).ToHtmlString(), "Oui"));
            if (displayOnTwoLines)
                stringBuilder.Append(Tag.Br);
            stringBuilder.Append(string.Format("<span style='white-space:nowrap;'>{0}{1}</span>", html.RadioButtonFor(expression, false).ToHtmlString(), "Non"));
            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        #endregion

        #region TextBox

        public static IHtmlString TextBoxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int size)
        {
            return html.TextBoxFor(expression, new {size});
        }

        #endregion

        #region Javascript utilities

        public static string JsonSerialize(this HtmlHelper htmlHelper, object value)
        {
            return new JavaScriptSerializer().Serialize(value);
        }

        #endregion

        #region Constants

        private const int DefaultPageSize = 20;
        private const int DefaultPageNumber = 1;
        private const string DefaultSeparator = " | ";
        private const string ControllerRouteValueKey = "controller";
        private const string ActionRouteValueKey = "action";
        private const string SubmitButtonType = "submit";
        private const string IdsToReuseKey = "__htmlPrefixScopeExtensions_IdsToReuse_";
        private const string FloatRightClass = "floatRight";
        private const string FloatRightBoldClass = "floatRightBold";
        private const string ScriptsKey = "scripts";
        private const string ConfirmDelete = "confirmDelete";

        #endregion

        #region ActionLink

        public static MvcHtmlString ActionLink<TController>(this HtmlHelper helper, string linkText, Expression<Action<TController>> action) where TController : Controller
        {
            return helper.ActionLink(linkText, action, null, null);
        }

        public static MvcHtmlString ActionLink<TController>(this HtmlHelper helper, string linkText, Expression<Action<TController>> action, object routeValues) where TController : Controller
        {
            return helper.ActionLink(linkText, action, routeValues, null);
        }

        public static MvcHtmlString ActionLink<TController>(this HtmlHelper helper, string linkText, Expression<Action<TController>> action, object routeValues, object htmlAttributes)
            where TController : Controller
        {
            Requires<ArgumentNullException>.IsNotNull(action);
            Requires<ArgumentNullException>.IsNotNull(linkText);
            Requires<ArgumentException>.GreaterThan(linkText.Length, 0);

            var routingValues = MvcExpressionHelper.GetRouteValuesFromExpression(action)
                                                   .Merge(new RouteValueDictionary(routeValues));

            return helper.RouteLink(linkText, routingValues, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString ConditionalPdfActionLink<TController>(this HtmlHelper helper,  bool condition, string linkText, Expression<Action<TController>> action, int carriageReturns = 0)
            where TController : Controller
        {
            
            return condition
                ? MvcHtmlString.Create(helper.ActionLink(linkText, action, null, new
                {
                    @class = "pdf"
                }) + string.Concat(Enumerable.Repeat("<br />", carriageReturns)))

                : null;
        }

        #endregion

        #region BeginForm

        public static MvcForm BeginForm<TController>(this HtmlHelper helper, Expression<Action<TController>> action) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(action);
            return helper.BeginForm(routeValues);
        }

        public static MvcForm BeginMultipartForm(this HtmlHelper htmlHelper)
        {
            return htmlHelper.BeginForm(null, null, FormMethod.Post, new Dictionary<string, object> {{"enctype", "multipart/form-data"}});
        }

        #endregion

        #region Pagination

        public static MvcHtmlString PaginationMenu(this HtmlHelper htmlHelper, string orderName, string direction, int totalElement, int currentPage, int pageSize)
        {
            if (totalElement == 0)
                return MvcHtmlString.Empty;

            //what does it mean negative values for params ???
            const int nrOfPagesToDisplay = 10;
            if (pageSize < 1)
                pageSize = DefaultPageSize;
            if (currentPage < 1)
                currentPage = DefaultPageNumber;

            var urldata = htmlHelper.ViewContext.RouteData.Values.ToList().ToDictionary(item => item.Key, item => item.Value.ToString());
            var querystring = htmlHelper.ViewContext.HttpContext.Request.QueryString;
            foreach (string key in querystring.Keys)
                if (!urldata.Keys.Contains(key))
                    urldata.Add(key, querystring[key]);

            var sb = new StringBuilder();
            if (currentPage > 1)
                sb.Append(htmlHelper.GeneratePageLink("<", urldata, 1, orderName, direction));

            var pageCount = (int) Math.Ceiling((double) totalElement/pageSize);
            var start = 1;
            var end = pageCount;
            if (pageCount > nrOfPagesToDisplay)
            {
                var middle = (int) Math.Ceiling(nrOfPagesToDisplay/2d) - 1;
                var below = currentPage - middle;
                var above = currentPage + middle;

                if (below < 4)
                {
                    above = nrOfPagesToDisplay;
                    below = 1;
                }
                else if (above > pageCount - 4)
                {
                    above = pageCount;
                    below = pageCount - nrOfPagesToDisplay;
                }

                start = below;
                end = above;
            }

            if (start > 3)
            {
                sb.Append(htmlHelper.GeneratePageLink("1", urldata, 1, orderName, direction));
                sb.Append(htmlHelper.GeneratePageLink("2", urldata, 2, orderName, direction));
                sb.Append("...").Append(DefaultSeparator);
            }

            for (var i = start; i <= end; i++)
            {
                if (i == currentPage || (currentPage <= 0 && i == 0))
                    sb.Append(i).Append(DefaultSeparator);
                else
                    sb.Append(htmlHelper.GeneratePageLink(i.ToString(), urldata, i, orderName, direction));
            }

            if (end < pageCount - 3)
            {
                sb.Append("...").Append(DefaultSeparator);
                sb.Append(htmlHelper.GeneratePageLink((pageCount - 1).ToString(), urldata, pageCount - 1, orderName, direction));
                sb.Append(htmlHelper.GeneratePageLink(pageCount.ToString(), urldata, pageCount, orderName, direction));
            }

            // Next
            sb.Append(currentPage < pageCount ? htmlHelper.GeneratePageLink(">", urldata, currentPage + 1, orderName, direction) : string.Empty);
            var pagination = sb.ToString();
            pagination = pagination.Substring(0, pagination.Length - Math.Min(DefaultSeparator.Length, pagination.Length));

            return MvcHtmlString.Create(pagination);
        }

        private static string GeneratePageLink(this HtmlHelper helper, string linkText, IDictionary<string, string> urldata, int page, string ordername, string direction)
        {
            var action = urldata[ActionRouteValueKey];
            var controller = urldata[ControllerRouteValueKey];
            var routeValues = new RouteValueDictionary(new {Page = page, Order = ordername, Direction = direction});
            foreach (var item in urldata.Where(m => !new[] {"controller", "action", "Page", "Order", "Direction"}.Contains(m.Key)))
            {
                if (!routeValues.ContainsKey(item.Key))
                    routeValues.Add(item.Key, item.Value);
                else
                    routeValues[item.Key] = item.Value;
            }

            return string.Format("{0}{1}",
                                 helper.ActionLink(linkText, action, controller, routeValues, null),
                                 DefaultSeparator);
        }

        #endregion

        #region ActionImage

        public static MvcHtmlString ActionImage(this HtmlHelper helper, string action, object routeValues, string imagePath, string alt)
        {
            return helper.ActionImage(action, null, routeValues, imagePath, alt);
        }

        public static MvcHtmlString ActionImage(this HtmlHelper helper, string action, string controller, object routeValues, string imagePath, string alternativeText)
        {
            return ImageLink(helper, action, controller, routeValues, imagePath, alternativeText);
        }

        public static MvcHtmlString ActionImage<TController>(this HtmlHelper helper, Expression<Action<TController>> action, string imageRelativeUrl, string alternativeText)
            where TController : Controller
        {
            return helper.ActionImage(action, imageRelativeUrl, alternativeText, null, null);
        }

        public static MvcHtmlString ActionImage<TController>(this HtmlHelper helper, Expression<Action<TController>> action, string imageRelativeUrl, string alternativeText, object htmlAttributes)
            where TController : Controller
        {
            return helper.ActionImage(action, imageRelativeUrl, alternativeText, null, htmlAttributes);
        }

        public static MvcHtmlString ActionImage<TController>(this HtmlHelper htmlHelper, Expression<Action<TController>> action, string relativeUrl, string altText, object routeValues, object htmlAttributes)
            where TController : Controller
        {
            Requires<ArgumentNullException>.IsNotNull(action);
            Requires<ArgumentNullException>.IsNotNull(relativeUrl);
            Requires<ArgumentException>.GreaterThan(relativeUrl.Length, 0);

            var routeValueDictionary = MvcExpressionHelper.GetRouteValuesFromExpression(action)
                                                          .Merge(new RouteValueDictionary(routeValues));
            var actionName = routeValueDictionary[MvcConstants.ActionRouteValueKey].ToString();
            var controllerName = routeValueDictionary[MvcConstants.ControllerRouteValueKey].ToString();
            return htmlHelper.ViewContext.ImageLink(actionName, controllerName, routeValueDictionary, relativeUrl, altText, htmlAttributes);
        }

        #region ActionImageHelpers

        public static MvcHtmlString ImageLink(HtmlHelper helper, string action, string controller, object routeValues, string imageRelativeUrl, string alternativeText)
        {
            return helper.ViewContext.ImageLink(action, controller, routeValues, imageRelativeUrl, alternativeText);
        }

        public static MvcHtmlString ImageLink(this ViewContext context, string action, string controller, object routeValues, string imageRelativeUrl, string alternativeText)
        {
            return context.ImageLink(action, controller, routeValues, imageRelativeUrl, alternativeText, null);
        }

        public static MvcHtmlString ImageLink(this ViewContext context, string action, string controller, object routeValues, string imageRelativeUrl, string alternativeText, object htmlAttributes)
        {
            var url = new UrlHelper(context.RequestContext);
            var urlPath = routeValues != null && routeValues is RouteValueDictionary ?
                              url.Action(action, controller, routeValues as RouteValueDictionary) :
                              url.Action(action, controller, routeValues);

            return Tag.A(urlPath)
                      .Html(Tag.Img(url.Content(imageRelativeUrl), alternativeText, alternativeText)
                               .SelfClose())
                      .MergeAttributes(IDictionaryParse<object>(htmlAttributes))
                      .ToMvcHtmlString();
        }

        #endregion

        #endregion

        #region SaveButton

        public static MvcHtmlString SaveButton(this HtmlHelper helper, string id = null, string name = null)
        {
            return helper.SubmitButton(name, id, AgrideaCoreStrings.WebSaveButtonCaption);
        }

        public static MvcHtmlString SaveButtonTop(this HtmlHelper helper, string id = null, string name = null)
        {
            return helper.SubmitButton(name, id, AgrideaCoreStrings.WebSaveButtonCaption, "top");
        }

        public static MvcHtmlString SaveButtonBottom(this HtmlHelper helper, string id = null, string name = null)
        {
            return helper.SubmitButton(name, id, AgrideaCoreStrings.WebSaveButtonCaption, "bottom");
        }

        #endregion

        #region DeleteButton

        public static MvcHtmlString DeleteButton(this HtmlHelper helper, string id = null, string name = null)
        {
            return helper.SubmitButton(name, id, AgrideaCoreStrings.WebDeleteButtonCaption);
        }

        public static MvcHtmlString DeleteImageButton(this HtmlHelper helper, string imagePath, string id = null, string name = null)
        {
            return helper.SubmitButton(name, id, AgrideaCoreStrings.WebDeleteButtonCaption, imagePath);
        }

        public static MvcHtmlString DeleteButtonForm<TController, TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Action<TController>> action, Expression<Func<TModel, TProperty>> hiddenProperty, object routeValues = null)
            where TController : Controller

            //where TModel : PocoBase
        {
            var routingValues = MvcExpressionHelper.GetRouteValuesFromExpression(action);
            var formAction = UrlHelper.GenerateUrl(null,
                                                   routingValues[ActionRouteValueKey].ToString(),
                                                   routingValues[ControllerRouteValueKey].ToString(),
                                                   new RouteValueDictionary(routeValues), helper.RouteCollection, helper.ViewContext.RequestContext, true);
            var form = Tag.Form;
            form.Class(ConfirmDelete);
            form.MergeAttribute(FluentTagBuilder.Method, "post")
                .MergeAttribute(FluentTagBuilder.Action, formAction);
            form.Html(helper.HiddenFor(hiddenProperty))
                .Html(Tag.P.Html(helper.DeleteButton()));

            return form.ToMvcHtmlString();
        }

        public static MvcHtmlString DeleteButtonForm<TController>(this HtmlHelper helper, Expression<Action<TController>> action, object routeValues)
            where TController : Controller
        {
            var routingValues = MvcExpressionHelper.GetRouteValuesFromExpression(action);
            var formAction = UrlHelper.GenerateUrl(null,
                                                   routingValues[ActionRouteValueKey].ToString(),
                                                   routingValues[ControllerRouteValueKey].ToString(),
                                                   new RouteValueDictionary(routeValues),
                                                   helper.RouteCollection,
                                                   helper.ViewContext.RequestContext,
                                                   true);
            var form = Tag.Form;
            form.Class(ConfirmDelete);
            form.MergeAttribute(FluentTagBuilder.Method, "post")
                .MergeAttribute(FluentTagBuilder.Action, formAction);
            form.Html(Tag.P.Html(helper.DeleteButton()));

            return form.ToMvcHtmlString();
        }

        #endregion

        #region BeginForm

        /// <summary>
        /// This methods assumes that we have a Search POST action result in the actual controller
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static MvcForm BeginSearchForm(this HtmlHelper helper)
        {
            var controllerName = helper.ViewContext.Controller.ControllerContext.RouteData.Values["controller"].ToString();
            return helper.BeginForm("Search", controllerName, FormMethod.Post, new
            {
                @class = "no-dirty"
            });
        }

        public static MvcForm BeginSearchForm(this HtmlHelper helper, string actionName)
        {
            var controllerName = helper.ViewContext.Controller.ControllerContext.RouteData.Values["controller"].ToString();
            return helper.BeginForm(actionName, controllerName, FormMethod.Post, new
            {
                @class = "no-dirty"
            });
        }

        public static MvcForm BeginForm(this HtmlHelper helper, object htmlAttributes = null)
        {
            return helper.BeginForm(null, null, FormMethod.Post, htmlAttributes);
        }

        public static MvcForm BeginForm(this HtmlHelper helper, IDictionary<string, object> htmlAttributes = null)
        {
            return helper.BeginForm(null, null, FormMethod.Post, htmlAttributes);
        }

        #endregion

        #region ListEditing

        public static IDisposable BeginCollectionItem(this HtmlHelper html, string collectionName)
        {
            var idsToReuse = GetIdsToReuse(html.ViewContext.HttpContext, collectionName);
            var itemIndex = idsToReuse.Count > 0 ? idsToReuse.Dequeue() : Guid.NewGuid().ToString();

            // autocomplete="off" is needed to work around a very annoying Chrome behaviour whereby it reuses old values after the user clicks "Back", which causes the xyz.index and xyz[...] values to get out of sync.
            html.ViewContext.Writer.WriteLine("<input type=\"hidden\" name=\"{0}.index\" autocomplete=\"off\" value=\"{1}\" />", collectionName, html.Encode(itemIndex));

            return BeginHtmlFieldPrefixScope(html, string.Format("{0}[{1}]", collectionName, itemIndex));
        }

        public static IDisposable BeginHtmlFieldPrefixScope(this HtmlHelper html, string htmlFieldPrefix)
        {
            return new HtmlFieldPrefixScope(html.ViewData.TemplateInfo, htmlFieldPrefix);
        }

        #region Helpers

        private static Queue<string> GetIdsToReuse(HttpContextBase httpContext, string collectionName)
        {
            // We need to use the same sequence of IDs following a server-side validation failure,  
            // otherwise the framework won't render the validation error messages next to each item.
            var key = IdsToReuseKey + collectionName;
            var queue = (Queue<string>) httpContext.Items[key];
            if (queue == null)
            {
                httpContext.Items[key] = queue = new Queue<string>();
                var previouslyUsedIds = httpContext.Request[collectionName + ".index"];
                if (!string.IsNullOrEmpty(previouslyUsedIds))
                    foreach (var previouslyUsedId in previouslyUsedIds.Split(','))
                        queue.Enqueue(previouslyUsedId);
            }
            return queue;
        }

        #endregion

        #region Private Classes

        private class HtmlFieldPrefixScope : IDisposable
        {
            private readonly string previousHtmlFieldPrefix_;
            private readonly TemplateInfo templateInfo_;

            public HtmlFieldPrefixScope(TemplateInfo templateInfo, string htmlFieldPrefix)
            {
                templateInfo_ = templateInfo;

                previousHtmlFieldPrefix_ = templateInfo.HtmlFieldPrefix;
                templateInfo.HtmlFieldPrefix = htmlFieldPrefix;
            }

            public void Dispose()
            {
                templateInfo_.HtmlFieldPrefix = previousHtmlFieldPrefix_;
            }
        }

        #endregion

        #endregion

        #region cols

        public static MvcHtmlString Columns(this HtmlHelper html, params int[] widths)
        {
            var colGroup = Tag.ColGroup;
            var widthSum = widths.Sum();
            Asserts<ArgumentException>.LessOrEqual(widthSum, 100);
            foreach (var width in widths)
                colGroup.Html(Tag.Col
                                 .Style(width.ToPercentStyleWidth()));

            return MvcHtmlString.Create(colGroup.ToString());
        }

        private static string ToPercentStyleWidth(this int width)
        {
            return string.Format("width:{0}%", width);
        }

        #endregion

        #region Numeric Format

        public static MvcHtmlString DisplayDecimalFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int digit = 2)
        {
            var content = Convert.ToDouble(html.DisplayTextFor(expression).ToString());
            var span = Tag.Span
                          .Style("float:right;")
                          .Text(content.ToString("N" + digit));
            return MvcHtmlString.Create(span.ToString());
        }

        public static MvcHtmlString DisplayDecimalTotalFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int digit = 2)
        {
            var content = Convert.ToDouble(html.DisplayTextFor(expression).ToString());
            var span = Tag.Span
                          .Style("float:right; font-weight:bold;")
                          .Text(content.ToString("N" + digit));
            return MvcHtmlString.Create(span.ToString());
        }

        #endregion

        #region AutoComplete ComboBox

        /// <summary>
        /// Creates a gracefully degradable autocomplete DropDownList. If minLength > 0, you cannot show all items
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="minLength"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static IHtmlString AutoCompleteDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, int width = 0, int minLength = 0)
        {
            return htmlHelper.AutoCompleteDropDownListFor(expression, selectList, null, width, minLength);
        }

        public static IHtmlString AutoCompleteDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes, int width = 0, int minLength = 0)
        {
            htmlAttributes = AddClass(htmlAttributes, "autocomplete");
            htmlAttributes.Add("autocomplete-minlength", minLength);
            htmlAttributes.Add("autocomplete-width", width);
            var dropDownList = htmlHelper.DropDownListFor(expression, selectList, null, htmlAttributes);
            var generatedId = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
            Requires<ArgumentNullException>.IsNotNull(generatedId);
            return MvcHtmlString.Create(dropDownList.ToHtmlString());
        }

        #endregion

        #region Partial

        public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string partialViewName)
        {
            return helper.PartialFor(expression, partialViewName, null);
        }

        public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string partialViewName, ViewDataDictionary viewData)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            var model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;

            var viewDataInternal = new ViewDataDictionary(helper.ViewData)
            {
                TemplateInfo = new TemplateInfo {HtmlFieldPrefix = CombineHtmlFieldPrefix(helper.ViewData.TemplateInfo, name)}
            };
            if (viewData != null)
                foreach (var item in viewData)
                {
                    object val = null;
                    if (viewDataInternal.TryGetValue(item.Key, out val))
                        viewDataInternal[item.Key] = item.Value;
                    else
                        viewDataInternal.Add(item.Key, item.Value);
                }
            return helper.Partial(partialViewName, model, viewDataInternal);
        }

        #endregion

        #region Calendar

        /// <summary>
        /// Creates a TextBox with a datePicker class. To relate to jquery  $.datepicker function... to display a nicd
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString CalendarFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, DateTime?>> expression)
        {
            var content = helper.DisplayTextFor(expression).ToString();
            var displayValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(content))
            {
                displayValue = Convert.ToDateTime(content).ToShortDateString();
            }
            return helper.TextBox(ExpressionHelper.GetExpressionText(expression), displayValue, new {@class = "datePicker"});
        }

        public static MvcHtmlString CalendarFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, DateTime>> expression, int? minRange = null)
        {
            var content = helper.DisplayTextFor(expression).ToString();
            if (content == default(DateTime).ToString())
                content = string.Empty;
            var displayValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(content))
            {
                displayValue = Convert.ToDateTime(content).ToShortDateString();
            }
            var attributes = new Dictionary<string, object>
            {
                {
                    "class", "datePicker"
                }
            };
            if (minRange != null)
                attributes.Add("min_year", minRange);

            return helper.TextBox(ExpressionHelper.GetExpressionText(expression), displayValue, attributes);
        }

        public static MvcHtmlString CalendarFor<TModel>(this HtmlHelper<TModel> helper, string dateFieldName)
        {
            var content = helper.DisplayText(dateFieldName).ToString();
            var displayValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(content))
                displayValue = Convert.ToDateTime(content).ToShortDateString();
            return helper.TextBox(dateFieldName, displayValue, new {@class = "datePicker"});
        }

        #endregion

        #region DateTime

        public static MvcHtmlString DisplayDateFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string dateFormat = "d")
        {
            var value = Convert.ToDateTime(expression.GetValue(html.ViewData));
            return MvcHtmlString.Create(value.ToString(dateFormat));
        }

        #endregion

        #region Helper

        public static string CombineHtmlFieldPrefix(TemplateInfo templateInfo, string prefix)
        {
            if (templateInfo == null)
                return prefix;
            if (string.IsNullOrEmpty(templateInfo.HtmlFieldPrefix))
                return prefix;
            if (string.IsNullOrEmpty(prefix))
                return templateInfo.HtmlFieldPrefix;

            if (prefix.IsIndexer())
                return string.Format("{0}{1}", templateInfo.HtmlFieldPrefix, prefix);
            return string.Format("{0}.{1}", templateInfo.HtmlFieldPrefix, prefix);
        }

        public static bool IsIndexer(this string prefix)
        {
            return new Regex(@"\[\d+\]").Match(prefix).Success;
        }

        public static IDictionary<string, object> AddClass(IDictionary<string, object> htmlAttributes, string className)
        {
            if (htmlAttributes == null)
            {
                var ret = new Dictionary<string, object>();
                ret.Add("class", className);
                return ret;
            }
            if (htmlAttributes.ContainsKey("class"))
            {
                var classList = (string) htmlAttributes["class"];
                classList.Append(" ").Append(className);
                return htmlAttributes;
            }
            htmlAttributes.Add("class", className);
            return htmlAttributes;
        }

        public static IDictionary<string, T> IDictionaryParse<T>(object withProperties) where T : class
        {
            IDictionary<string, T> dic = new Dictionary<string, T>();
            var properties = TypeDescriptor.GetProperties(withProperties);
            foreach (PropertyDescriptor property in properties)
            {
                dic.Add(property.Name, property.GetValue(withProperties) as T);
            }
            return dic;
        }

        #endregion

        #region ProgressBar

        public static IHtmlString ProgressBarFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, double>> expression, bool showValue = true)
        {
            double value;
            if (!double.TryParse(helper.DisplayFor(expression).ToString(), out value))
                return null;
            return helper.ProgressBarFor(value, showValue);
        }

        public static IHtmlString ProgressBarFor<TModel>(this HtmlHelper<TModel> helper, double value, bool showValue = true)
        {
            var guid = Guid.NewGuid().ToString();
            var div = Tag.Div.Id(guid).Html(Tag.Span.MergeAttribute("style", "position:absolute; margin-left:10px; margin-top:3px; font-size:11px;").Html(Math.Round(value, 2) + " %"));
            var sb = new StringBuilder();
            sb.Append(div);
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine("$(function() {");
            sb.AppendLine("$('#" + guid + "').progressbar({");
            sb.AppendLine().AppendFormat("value:{0}", value);
            sb.AppendLine("});");
            sb.AppendLine("});");
            sb.AppendLine("</script>");
            return MvcHtmlString.Create(sb.ToString());
        }

        #endregion

        #region Iban

        public static IHtmlString IbanFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, string>> expressionForIban)
        {
            return helper.TextBoxFor(expressionForIban, new {@class = "iban"});
        }

        public static IHtmlString IbanFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, string>> expressionForIban, object htmlAttributes)
        {
            var htmlAttr = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            htmlAttr = htmlAttr.AddClass("iban");
            return helper.TextBoxFor(expressionForIban, htmlAttr);
        }

        #endregion Iban

        #region Helpers

        public static object GetValue<TModel, TProperty>(this Expression<Func<TModel, TProperty>> expression, ViewDataDictionary<TModel> viewData)
        {
            return ModelMetadata.FromLambdaExpression(expression, viewData).Model;
        }

        #endregion
    }
}