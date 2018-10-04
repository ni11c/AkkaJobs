using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ajax.Utilities;

namespace System.Web.Mvc
{
    public class PartialScriptInclude
    {
        public string Path { get; set; }
        public int Priority { get; set; }
        public string Source { get; set; }
    }

    public static class PartialScriptIncludeHtmlHelper
    {
        public static string RequireScriptBlock(this HtmlHelper htmlHelper, string source, int priority = 1)
        {
            var requiredScripts = HttpContext.Current.Items["RequiredScripts"] as List<PartialScriptInclude>;
            if (requiredScripts == null)
                HttpContext.Current.Items["RequiredScripts"] = requiredScripts = new List<PartialScriptInclude>();

            Minifier minifier = new Minifier();
            string minified = minifier.MinifyJavaScript(source);
            if (minifier.Errors.Count > 0)
                return null;
            
            if (requiredScripts.All(i => i.Source != minified))
                requiredScripts.Add(new PartialScriptInclude() { Source = minified, Priority = priority });

            return null;
        }
        public static string RequireScript(this HtmlHelper html, string path, int priority = 1)
        {
            var requiredScripts = HttpContext.Current.Items["RequiredScripts"] as List<PartialScriptInclude>;
            if (requiredScripts == null)
                HttpContext.Current.Items["RequiredScripts"] = requiredScripts = new List<PartialScriptInclude>();

            if (requiredScripts.All(i => i.Path != path))
                requiredScripts.Add(new PartialScriptInclude() { Path = path, Priority = priority });

            return null;
        }
        public static HtmlString EmitRequiredScriptsFromPartials(this HtmlHelper html)
        {
            var requiredScripts = HttpContext.Current.Items["RequiredScripts"] as List<PartialScriptInclude>;
            if (requiredScripts == null)
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (var item in requiredScripts.OrderByDescending(i => i.Priority))
            {
                if (!string.IsNullOrWhiteSpace(item.Source))
                    sb.AppendFormat("<script type=\"text/javascript\">\n{0}\n</script>\n", item.Source);
                else
                    sb.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>\n", item.Path);
            }
            return new HtmlString(sb.ToString());
        }
    }
}
