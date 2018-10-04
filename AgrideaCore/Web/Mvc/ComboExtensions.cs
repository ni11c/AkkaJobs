using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Agridea.Diagnostics.Contracts;

namespace Agridea.Web.Mvc
{
    public static class ComboExtensions
    {
        #region Services
        public static IEnumerable<SelectListItem> ToSelectListItem<T, TValue, TText>(
            this IEnumerable<T> enumerable,
            Func<T, TText> text,
            Func<T, TValue> value,
            Func<T, SelectListGroup> group = null)
        {
            return enumerable.Select(item => new SelectListItem
            {
                Text = text(item).ToString(),
                Value = value(item).ToString(),
                Group = group != null ? group(item) : null
            }).AsEnumerable();
        }
        public static IEnumerable<SelectListItem> SetSelectedValue(
            this IEnumerable<SelectListItem> selectList,
            string selectedValue)
        {
            var updatedSelectList = new List<SelectListItem>();
            foreach (var item in selectList)
            {
                item.Selected = item.Value == selectedValue;
                updatedSelectList.Add(item);
            }
            return updatedSelectList;
        }

        public static IEnumerable<SelectListItem> ToSelectListItem<T, TValue, TText>(this IEnumerable<T> enumerable,
            Func<T, TText> text,
            Func<T, TValue> value,
            TValue selectedValue,
            Func<T, SelectListGroup> group = null)
        {
            Requires<ArgumentException>.AreEqual(typeof(TValue), typeof(TValue));
            return enumerable.Select(item => new SelectListItem
            {
                Text = text(item).ToString(),
                Value = value(item).ToString(),
                Selected = value(item).Equals(selectedValue),
                Group = group != null ? group(item) : null
            }).AsEnumerable();
        }

        public static IEnumerable<SelectListItem> WithDefaultZeroValue(this IEnumerable<SelectListItem> selectListItems, string chooseText/* = ChooseText*/)
        {
            IList<SelectListItem> items = selectListItems.ToList();
            items.Insert(0, new SelectListItem { Value = "0", Text = chooseText });

            return items.AsEnumerable();
        }
        public static IEnumerable<SelectListItem> WithDefaultValue(this IEnumerable<SelectListItem> selectListItems, string chooseText, string defaultValue/* = ChooseText*/)
        {
            IList<SelectListItem> items = selectListItems.ToList();
            items.Insert(0, new SelectListItem { Value = defaultValue, Text = chooseText });

            return items.AsEnumerable();
        }
        public static IEnumerable<SelectListItem> WithDefaultNullValue(this IEnumerable<SelectListItem> selectListItems, string chooseText/* = ChooseText*/)
        {
            IList<SelectListItem> items = selectListItems.ToList();
            items.Insert(0, new SelectListItem { Value = string.Empty, Text = chooseText });

            return items.AsEnumerable();
        }
        #endregion
    }
}
