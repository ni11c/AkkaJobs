using System;
using System.Collections.Generic;
using System.Web;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public interface IGridEditColumn<T> : IGridBoundColumn<T>
    {
        string Classes { get; set; }

        bool IsDisabled { get; set; }

        Func<T, bool> ReadOnlyFunc { get; set; }

        bool IsReadOnly { get; set; }

        int? InputSize { get; set; }

        IHtmlString RenderInput(T dataItem);

        bool IsHidden { get; set; }

        int? StyleWidth { get; set; }
        string PlaceHolder { get; set; }

    }
}