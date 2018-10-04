using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Agridea.Web.Mvc.Grid.Command
{
    public interface ICommand<T>
    {
        RouteValueDictionary RouteValueDictionary { get; }
        string AlternateText { get; set; }
        string ImageUrl { get; }
        object HtmlAttributes { get; }
        bool TextOnly { get; set; }
        Func<T, bool> HideWhenFunc { get; set; }
        bool IsHidden { get; set; }
        string Render(T dataItem, IList<IGridDataKey<T>> keys, ControllerContext context );
    }

}
