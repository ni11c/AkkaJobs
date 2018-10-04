using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class CustomHeader
    {
        public CustomHeader(string title, int colspan = 1)
        {
            Title = title;
            Colspan = colspan;
        }

        public string Title { get; set; }
        public int Colspan { get; set; }

        public IHtmlString Render()
        {
            return Tag.Th.Colspan(Colspan).Html(Title).ToMvcHtmlString();
        }
    }
}
