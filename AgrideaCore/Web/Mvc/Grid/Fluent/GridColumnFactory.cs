using Agridea.Diagnostics.Contracts;
using Agridea.Web.Mvc.Grid.Columns;
using Agridea.Web.Mvc.Grid.Command;
using Agridea.Web.UI;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public class GridColumnFactory<T>
    {
        public GridColumnFactory(GridModel<T> grid)
        {
            GridModel = grid;
        }

        public GridModel<T> GridModel { get; private set; }

        public GridBoundColumnBuilder<T> Bound<TValue>(string name, Func<T, TValue> func)
        {
            var column = new GridBoundColumn<T, TValue>(GridModel, name, func);
            GridModel.ColumnList.Add(column);
            return new GridBoundColumnBuilder<T>(column);
        }

        public GridBoundColumnBuilder<T> DisplayList<TValue>(string name, Func<T, IEnumerable<TValue>> func, string separator = null)
        {
            var column = new GridListColumn<T, TValue>(GridModel, name, func, separator);
            GridModel.ColumnList.Add(column);
            return new GridBoundColumnBuilder<T>(column);
        }

        public GridActionColumnBuilder<T> Command(Action<GridActionCommandFactory<T>> commandAction)
        {
            var column = new GridActionColumn<T>(GridModel);
            commandAction(new GridActionCommandFactory<T>(column, GridModel.Context));
            GridModel.ColumnList.Add(column);
            return new GridActionColumnBuilder<T>(column);
        }

        public GridEditColumnBuilder<T> TextBoxFor<TValue>(string name, Func<T, TValue> func)
        {
            return TextBoxFor(name, func, null);
        }

        public GridEditColumnBuilder<T> TextBoxFor<TValue>(string name, Func<T, TValue> func, int? inputSize)
        {
            var column = new GridTextBoxColumn<T, TValue>(GridModel, name, func, inputSize);
            GridModel.ColumnList.Add(column);
            return new GridEditColumnBuilder<T>(column);
        }

        public GridEditColumnBuilder<T> DropDownListFor<TValue>(string name, Func<T, TValue> func, Func<T, IEnumerable<SelectListItem>> items)
        {
            var column = new GridComboColumn<T, TValue>(GridModel, name, func, items);
            GridModel.ColumnList.Add(column);
            return new GridEditColumnBuilder<T>(column);
        }

        public GridEditColumnBuilder<T> DropDownListFor<TValue>(string name, Func<T, TValue> func, IEnumerable<SelectListItem> items)
        {
            var column = new GridComboColumn<T, TValue>(GridModel, name, func, items);
            GridModel.ColumnList.Add(column);
            return new GridEditColumnBuilder<T>(column);
        }

        public GridEditColumnBuilder<T> CheckBoxFor(string name, Func<T, bool> func)
        {
            var column = new GridCheckBoxColumn<T>(GridModel, name, func);
            GridModel.ColumnList.Add(column);
            return new GridEditColumnBuilder<T>(column);
        }

        public GridEditColumnBuilder<T> EditorFor<TValue>(string name, Func<T, TValue> func)
        {
            var column = new GridEditorColumn<T, TValue>(GridModel, name, func);
            GridModel.ColumnList.Add(column);
            return new GridEditColumnBuilder<T>(column);
        }

        public GridEditColumnBuilder<T> HiddenFor<TValue>(string name, Func<T, TValue> func)
        {
            var column = new GridHiddenColumn<T, TValue>(GridModel, name, func)
            {
                IsVisible = false,
                IsHidden = false
            };
            GridModel.ColumnList.Add(column);
            GridModel.HiddenColumns.Add(column);
            return new GridEditColumnBuilder<T>(column);
        }

        public GridEditColumnBuilder<T> CalendarFor(string name, Func<T, DateTime?> func)
        {
            var column = new GridCalendarNullableColumn<T>(GridModel, name, func);
            GridModel.ColumnList.Add(column);
            return new GridEditColumnBuilder<T>(column);
        }

        public GridEditColumnBuilder<T> CalendarFor(string name, Func<T, DateTime> func, int? minRange = null)
        {
            var column = new GridCalendarColumn<T>(GridModel, name, func, minRange);
            GridModel.ColumnList.Add(column);
            return new GridEditColumnBuilder<T>(column);
        }

        public GridBoundColumnBuilder<T> ActionLinkFor<TController, TValue>(string name, Func<T, TValue> func, Expression<Action<TController>> actionResult)
            where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var column = new GridActionLinkColumn<T, TValue>(GridModel, name, func);
            column.Command = new GridActionLinkCommand<T>(GridModel.Context, routeValues);
            GridModel.ColumnList.Add(column);
            return new GridBoundColumnBuilder<T>(column);
        }

        public GridBoundColumnBuilder<T> CheckEditFor<TController>(string name, Func<T, bool> checkValueFunc, Func<T, bool> isAllowedFunc, Expression<Action<TController>> actionResult)
            where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridCheckEditActionCommand<T>(GridModel.Context, GridClass.EditIcon, GridParameters.Edit, routeValues, null, isAllowedFunc, checkValueFunc);
            var column = new GridCheckEditColumn<T, bool>(GridModel, command, name, checkValueFunc);
            GridModel.ColumnList.Add(column);
            return new GridBoundColumnBuilder<T>(column);
        }

        public GridBoundColumnBuilder<T> CheckEditFor<TController>(string name, Func<T, bool> checkValueFunc, Expression<Action<TController>> actionResult)
            where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridCheckEditActionCommand<T>(GridModel.Context, GridClass.EditIcon, GridParameters.Edit, routeValues, null, m => true, checkValueFunc);
            var column = new GridCheckEditColumn<T, bool>(GridModel, command, name, checkValueFunc);
            GridModel.ColumnList.Add(column);
            return new GridBoundColumnBuilder<T>(column);
        }

        public GridBoundColumnBuilder<T> EditionState()
        {
            Requires<ArgumentException>.IsTrue(typeof(ITrackEdition).IsAssignableFrom(typeof(T)), string.Format("{0} should Implement {1}", typeof(T).Name, typeof(ITrackEdition).Name));
            var column = new GridStatusColumn<T>(GridModel, null);
            GridModel.ColumnList.Add(column);
            return new GridBoundColumnBuilder<T>(column);
        }

        public GridBoundColumnBuilder<T> EditionStateWithTooltip()
        {
            Requires<ArgumentException>.IsTrue(typeof(ITrackEdition).IsAssignableFrom(typeof(T)), string.Format("{0} should Implement {1}", typeof(T).Name, typeof(ITrackEdition).Name));
            var routeValues = GridModel.Context.GetRouteValueDictionary(GridParameters.ToolTipMethod, true);
            var column = new GridStatusColumn<T>(GridModel, routeValues);
            GridModel.ColumnList.Add(column);
            return new GridBoundColumnBuilder<T>(column);
        }

        public GridBoundColumnBuilder<T> EditionStateWithTooltip<TController>(Expression<Action<TController>> toolTipResult)
            where TController : Controller
        {
            Requires<ArgumentException>.IsTrue(typeof(ITrackEdition).IsAssignableFrom(typeof(T)), string.Format("{0} should Implement {1}", typeof(T).Name, typeof(ITrackEdition).Name));
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(toolTipResult);
            var column = new GridStatusColumn<T>(GridModel, routeValues);
            GridModel.ColumnList.Add(column);
            return new GridBoundColumnBuilder<T>(column);
        }

        public GridBoundColumnBuilder<T> CheckImageFor(string name, Func<T, bool> func)
        {
            var column = new GridCheckImageColumn<T>(GridModel, name, func);
            GridModel.ColumnList.Add(column);
            return new GridBoundColumnBuilder<T>(column);
        }

        public GridActionColumnBuilder<T> Custom(Func<T, string> customContent)
        {
            var column = new GridCustomColumn<T>(GridModel, customContent);
            GridModel.ColumnList.Add(column);
            return new GridActionColumnBuilder<T>(column);
        }
    }
}