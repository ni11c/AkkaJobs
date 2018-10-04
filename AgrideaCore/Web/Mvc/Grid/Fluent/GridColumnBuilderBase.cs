using Agridea.Diagnostics.Contracts;
using Agridea.Web.Mvc.Grid.Columns;
using System;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public abstract class GridColumnBuilderBase<TColumn, TColumnBuilder> : IHideObjectMembers
        where TColumnBuilder : GridColumnBuilderBase<TColumn, TColumnBuilder>
        where TColumn : IGridColumn
    {
        protected GridColumnBuilderBase(TColumn column)
        {
            Column = column;
        }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>The column.</value>
        public TColumn Column
        {
            get;
            private set;
        }

        public TColumnBuilder Title(string text)
        {
            Column.Title = text;

            return this as TColumnBuilder;
        }
        /// <summary>
        /// Set the column width in percent
        /// </summary>
        /// <param name="width">percentage</param>
        /// <returns></returns>
        public TColumnBuilder Width(int width)
        {
            Column.Width = width;
            return this as TColumnBuilder;
        }

        public TColumnBuilder Visible(bool value)
        {
            Column.IsVisible = value;
            return this as TColumnBuilder;
        }

        public TColumnBuilder VisibleForExport(bool value)
        {
            Column.IsVisibleForExport = value;
            return this as TColumnBuilder;
        }

        public TColumnBuilder HeaderRowspan(int value)
        {
            Asserts<ArgumentOutOfRangeException>.InRange(value, 1, 2, "Header rowspan must have a value of 1 or 2");
            Column.Rowspan = value;
            return this as TColumnBuilder;
        }

        public TColumnBuilder CustomHeader(string title, int colspan = 1)
        {
            Column.CustomHeader = new CustomHeader(title, colspan);
            return this as TColumnBuilder;
        }

        public TColumnBuilder NoTitle()
        {
            Column.HasTitle = false;
            return this as TColumnBuilder;
        }

        public TColumnBuilder IsMerged()
        {
            Column.IsMerged = true;
            return this as TColumnBuilder;
        }

        public TColumnBuilder CssClass(params string[] cssClass)
        {
            Column.CssClasses.AddRange(cssClass);
            return this as TColumnBuilder;
        }

        public TColumnBuilder HasColor()
        {
            Column.CssClasses.Add(GridClass.HasColor);
            return this as TColumnBuilder;
        }
    }
}