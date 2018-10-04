namespace System.Web.Mvc
{
    //public static class SelectListExtensions
    //{
    //    #region Constants
    //    private const string Choose = "--choisir--";
    //    #endregion

    //    #region Services
    //    /// <summary>
    //    /// Constructs a SelectList from an IEnumerable using lambda methods instead of strings (the SelectList
    //    /// constructors use strings to designate text and value fields)
    //    /// </summary>
    //    public static SelectList ToSelectList<T>(
    //        this IEnumerable<T> enumerable,
    //        Func<T, string> text,
    //        Func<T, string> value,
    //        string selectedValue = null
    //    )
    //    {
    //        {
    //            return new SelectList(enumerable
    //                .Select(item => new { Text = text(item), Value = value(item), SelectedValue = selectedValue }),
    //                "Value",
    //                "Text"
    //            );
    //        }
    //    }
    //    public static SelectList ToSelectList<T>(
    //        this IEnumerable<T> enumerable,
    //        Func<T, string> text,
    //        Func<T, int> value,
    //        int selectedValue
    //        )
    //    {
    //        {
    //            return new SelectList(enumerable
    //                .Select(item => new { Text = text(item), Value = value(item).ToString(), selectedValue }),
    //            "Value",
    //            "Text"
    //                );
    //        }
    //    }

    //    public static SelectList WithDefaultZeroValue(this SelectList selectList, string chooseText = Choose)
    //    {
    //        IList<SelectListItem> list = selectList.ToList();
    //        list.Insert(0, new SelectListItem { Value = "0", Text = chooseText });
    //        return list.ToSelectList(x => x.Text, x => x.Value);
    //    }
    //    public static SelectList WithDefaultNullValue(this SelectList selectList, string chooseText = Choose)
    //    {
    //        IList<SelectListItem> list = selectList.ToList();
    //        list.Insert(0, new SelectListItem { Value = null, Text = chooseText });
    //        return list.ToSelectList(x => x.Text, x => x.Value);
    //    }
    //    #endregion
    //}
}