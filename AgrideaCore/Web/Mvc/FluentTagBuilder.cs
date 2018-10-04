using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Agridea.Web.Mvc
{
    public class FluentTagBuilder
    {
        #region Constants
        public const string Html = "html";

        #region Miscellaneous
        public const string Div = "div";
        public const string P = "p";
        public const string Span = "span";
        public const string Strong = "strong";
        public const string B = "b";
        public const string Br = "br";
        public const string Label = "label";
        #endregion

        #region Links
        public const string A = "a";
        #endregion

        #region Images
        public const string Img = "img";
        #endregion

        #region Forms
        public const string Form = "form";
        public const string Input = "input";
        #endregion

        #region Combo

        public const string Select = "select";
        public const string Option = "option";
        #endregion

        #region Table
        public const string Table = "table";
        public const string Tbody = "tbody";
        public const string Thead = "thead";
        public const string Tr = "tr";
        public const string Td = "td";
        public const string Th = "th";
        public const string ColGroup = "colgroup";
        public const string Col = "col";
        #endregion

        #region Lists
        public const string Ol = "ol";
        public const string Ul = "ul";
        public const string Li = "li";
        #endregion

        #region Attributes
        public const string Href = "href";
        public const string Alt = "alt";
        public const string Title = "title";
        public const string Src = "src";
        public const string Type = "type";
        public const string Value = "value";
        public const string Name = "name";
        public const string Class = "class";
        public const string Style = "style";
        public const string Id = "id";
        public const string For = "for";
        public const string Colspan = "colspan";
        public const string Rowspan = "rowspan";
        public const string Width = "width";
        public const string Method = "method";
        public const string Action = "action";
        public const string Selected = "selected";
        #endregion

        #endregion

        #region Members
        private readonly TagBuilder tagBuilder_;
        #endregion

        #region Initialization
        public FluentTagBuilder(string tagName)
        {
            tagBuilder_ = new TagBuilder(tagName);
        }
        #endregion

        #region Properties
        public IDictionary<string, string> Attributes
        {
            get { return tagBuilder_.Attributes; }
        }
        public string IdAttributeDotReplacement
        {
            get { return tagBuilder_.IdAttributeDotReplacement; }
            set { tagBuilder_.IdAttributeDotReplacement = value; }
        }
        public string InnerHtml
        {
            get { return tagBuilder_.InnerHtml; }
            set { tagBuilder_.InnerHtml = value; }
        }
        public string TagName { get { return tagBuilder_.TagName; } }
        #endregion

        #region Methods
        public FluentTagBuilder AddCssClass(string value, bool condition = true)
        {
            if (condition)
                tagBuilder_.AddCssClass(value);
            return this;
        }
        public FluentTagBuilder GenerateId(string name)
        {
            tagBuilder_.GenerateId(name);
            return this;
        }
        public FluentTagBuilder MergeAttribute(string key, string value)
        {
            tagBuilder_.MergeAttribute(key, value);
            return this;
        }
        public FluentTagBuilder MergeAttribute(string key, int value)
        {
            tagBuilder_.MergeAttribute(key, value.ToString(CultureInfo.InvariantCulture));
            return this;
        }
        public FluentTagBuilder MergeAttribute(string key, string value, bool replaceExisting)
        {
            tagBuilder_.MergeAttribute(key, value, replaceExisting);
            return this;
        }
        public FluentTagBuilder MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes)
        {
            tagBuilder_.MergeAttributes(attributes);
            return this;
        }
        public FluentTagBuilder MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes, bool replaceExisting)
        {
            tagBuilder_.MergeAttributes(attributes, replaceExisting);
            return this;
        }
        public FluentTagBuilder SetInnerText(string innerText)
        {
            tagBuilder_.SetInnerText(innerText);
            return this;
        }
        public FluentTagBuilder AddInnerHtml(MvcHtmlString innerHtml)
        {
            tagBuilder_.InnerHtml += innerHtml.ToHtmlString();
            return this;
        }
        public FluentTagBuilder AddInnerHtml(string innerHtml)
        {
            tagBuilder_.InnerHtml += innerHtml;
            return this;
        }
        public FluentTagBuilder AddInnerHtml(FluentTagBuilder tagBuilder)
        {
            if (tagBuilder != null)
                tagBuilder_.InnerHtml += tagBuilder.ToString();
            return this;
        }
        public FluentTagBuilder AddInnerHtml(params FluentTagBuilder[] tags)
        {
            foreach (var tag in tags)
                AddInnerHtml(tag);
            return this;
        }
        public override string ToString()
        {
            return tagBuilder_.ToString();
        }
        public string ToString(TagRenderMode renderMode)
        {
            return tagBuilder_.ToString(renderMode);
        }
        #endregion
    }




    public static class Tag
    {
        #region table
        public static FluentTagBuilder Table { get { return new FluentTagBuilder(FluentTagBuilder.Table); } }
        public static FluentTagBuilder Tbody { get { return new FluentTagBuilder(FluentTagBuilder.Tbody); } }
        public static FluentTagBuilder Thead { get { return new FluentTagBuilder(FluentTagBuilder.Thead); } }
        public static FluentTagBuilder Td { get { return new FluentTagBuilder(FluentTagBuilder.Td); } }
        public static FluentTagBuilder Th { get { return new FluentTagBuilder(FluentTagBuilder.Th); } }
        public static FluentTagBuilder Col { get { return new FluentTagBuilder(FluentTagBuilder.Col); } }
        public static FluentTagBuilder ColGroup { get { return new FluentTagBuilder(FluentTagBuilder.ColGroup); } }
        #endregion

        public static string Br { get { return new FluentTagBuilder(FluentTagBuilder.Br).ToString(TagRenderMode.SelfClosing); } }
        public static FluentTagBuilder Tr { get { return new FluentTagBuilder(FluentTagBuilder.Tr); } }
        public static FluentTagBuilder Form { get { return new FluentTagBuilder(FluentTagBuilder.Form).MergeAttribute("method", "post"); } }
        public static FluentTagBuilder Div { get { return new FluentTagBuilder(FluentTagBuilder.Div); } }
        public static FluentTagBuilder P { get { return new FluentTagBuilder(FluentTagBuilder.P); } }
        public static FluentTagBuilder Span { get { return new FluentTagBuilder(FluentTagBuilder.Span); } }
        public static FluentTagBuilder B { get { return new FluentTagBuilder(FluentTagBuilder.B); } }
        public static FluentTagBuilder Label { get { return new FluentTagBuilder(FluentTagBuilder.Label); } }
        public static FluentTagBuilder Ul { get { return new FluentTagBuilder(FluentTagBuilder.Ul); } }
        public static FluentTagBuilder Li { get { return new FluentTagBuilder(FluentTagBuilder.Li); } }
        public static FluentTagBuilder Select { get { return new FluentTagBuilder(FluentTagBuilder.Select); } }
        public static FluentTagBuilder Option(string key, string value, bool isSelected)
        {
            var option = new FluentTagBuilder(FluentTagBuilder.Option);
            option.MergeAttribute(FluentTagBuilder.Value, key);
            option.SetInnerText(value);
            if (isSelected)
                option.MergeAttribute(FluentTagBuilder.Selected, "selected");
            return option;

        }
        public static FluentTagBuilder A(string href)
        {
            return new FluentTagBuilder(FluentTagBuilder.A)
                .Href(href);
        }
        public static FluentTagBuilder Img(string src, string alt = "", string title = "")
        {
            return new FluentTagBuilder(FluentTagBuilder.Img)
                .Src(src)
                .Alt(alt)
                .Title(title);
        }
        public static string SelfClose(this FluentTagBuilder tag)
        {
            return tag.ToString(TagRenderMode.SelfClosing);
        }
        public static FluentTagBuilder Input(string type = null, string value = null)
        {
            var input = new FluentTagBuilder(FluentTagBuilder.Input);
            if (type != null)
                input.Type(type);
            if (value != null)
                input.Value(value);
            return input;
        }
        public static FluentTagBuilder Href(this FluentTagBuilder tag, string href)
        {
            return tag.MergeAttribute(FluentTagBuilder.Href, href);
        }
        public static FluentTagBuilder Src(this FluentTagBuilder tag, string src)
        {
            return tag.MergeAttribute(FluentTagBuilder.Src, src);
        }
        public static FluentTagBuilder Alt(this FluentTagBuilder tag, string alt)
        {
            return tag.MergeAttribute(FluentTagBuilder.Alt, alt);
        }
        public static FluentTagBuilder Title(this FluentTagBuilder tag, string title)
        {
            return tag.MergeAttribute(FluentTagBuilder.Title, title);
        }
        public static FluentTagBuilder Type(this FluentTagBuilder tag, string type)
        {
            return tag.MergeAttribute(FluentTagBuilder.Type, type);
        }
        public static FluentTagBuilder Value(this FluentTagBuilder tag, string value)
        {
            return tag.MergeAttribute(FluentTagBuilder.Value, value);
        }
        public static FluentTagBuilder Name(this FluentTagBuilder tag, string name)
        {
            return tag.MergeAttribute(FluentTagBuilder.Name, name);
        }
        public static FluentTagBuilder Name(this FluentTagBuilder tag, MvcHtmlString name)
        {
            return tag.Name(name.ToHtmlString());
        }
        public static FluentTagBuilder Colspan(this FluentTagBuilder tag, int span)
        {
            return tag.MergeAttribute(FluentTagBuilder.Colspan, span);
        }
        public static FluentTagBuilder Rowspan(this FluentTagBuilder tag, int span)
        {
            return tag.MergeAttribute(FluentTagBuilder.Rowspan, span);
        }
        public static FluentTagBuilder Bold(this FluentTagBuilder tag, string innerText)
        {
            return tag.AddInnerHtml(B.SetInnerText(innerText));
        }
        public static FluentTagBuilder Html(this FluentTagBuilder tag, IHtmlString innerHtml)
        {
            return tag.Html(innerHtml.ToHtmlString());
        }
        public static FluentTagBuilder Html(this FluentTagBuilder tag, string innerHtml)
        {
            return tag.AddInnerHtml(innerHtml);
        }
        public static FluentTagBuilder Html(this FluentTagBuilder tag, FluentTagBuilder content)
        {
            return tag.AddInnerHtml(content);
        }
        public static FluentTagBuilder Html(this FluentTagBuilder tag, params FluentTagBuilder[] tags)
        {
            foreach (var innerTag in tags)
                tag.Html(innerTag);
            return tag;
        }
        public static FluentTagBuilder Text(this FluentTagBuilder tag, string innerText)
        {
            return tag.SetInnerText(innerText);
        }
        public static FluentTagBuilder Text(this FluentTagBuilder tag, int innerText)
        {
            return tag.Text(innerText.ToString(CultureInfo.InvariantCulture));
        }
        public static FluentTagBuilder Id(this FluentTagBuilder tag, string id)
        {
            return tag.MergeAttribute(FluentTagBuilder.Id, id);
        }
        public static FluentTagBuilder Id(this FluentTagBuilder tag, MvcHtmlString id)
        {
            return tag.Id(id.ToHtmlString());
        }
        public static FluentTagBuilder For(this FluentTagBuilder tag, string forContent)
        {
            return tag.MergeAttribute(FluentTagBuilder.For, forContent);
        }
        public static FluentTagBuilder Id(this FluentTagBuilder tag, int id)
        {
            return tag.Id(id.ToString());
        }
        public static FluentTagBuilder Style(this FluentTagBuilder tag, string style)
        {
            return tag.MergeAttribute(FluentTagBuilder.Style, style);
        }
        public static FluentTagBuilder Style(this FluentTagBuilder tag, params string[] styles)
        {
            var stringBuilder = new StringBuilder();
            foreach (var style in styles)
                stringBuilder.AppendFormat("{0};", style);
            return tag.Style(stringBuilder.ToString());
        }
        public static FluentTagBuilder Class(this FluentTagBuilder tag, string className, bool condition = true)
        {
            return tag.AddCssClass(className, condition);
        }
        public static FluentTagBuilder Class(this FluentTagBuilder tag, params string[] className)
        {
            if (className.Length == 0) return tag;
            var classes = string.Join(" ", className);
            return tag.Class(classes);
        }
        public static MvcHtmlString ToMvcHtmlString(this FluentTagBuilder tag)
        {
            return MvcHtmlString.Create(tag.ToString());
        }
        public static MvcHtmlString ToMvcHtmlString(this FluentTagBuilder tag, TagRenderMode tagRenderMode)
        {
            return MvcHtmlString.Create(tag.ToString(tagRenderMode));
        }
    }
}
