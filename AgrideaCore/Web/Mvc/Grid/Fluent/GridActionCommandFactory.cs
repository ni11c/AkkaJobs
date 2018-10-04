using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Agridea.Web.Mvc.Grid.Columns;
using Agridea.Web.Mvc.Grid.Command;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public class GridActionCommandFactory<T> : IHideObjectMembers
    {
        protected GridActionColumn<T> Column { get; set; }
        protected ControllerContext Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridActionCommandFactory&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="column">The grid column.</param>
        /// <param name="context">ViewContext</param>
        public GridActionCommandFactory(GridActionColumn<T> column, ControllerContext context)
        {
            Column = column;
            Context = context;
        }

        public GridActionCommandBuilder<T> Edit<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridActionCommand<T>(Context, GridClass.EditIcon, GridParameters.Edit, routeValues, null);
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> Edit(string actionName = GridParameters.EditMethod)
        {
            var routeValues = Context.GetRouteValueDictionary(actionName, false);
            var command = new GridActionCommand<T>(Context, GridClass.EditIcon, GridParameters.Edit, routeValues, null);
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> UnDelete<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridActionCommand<T>(Context, GridClass.UnDeleteIcon, GridParameters.UnDelete, routeValues, new {@class = GridClass.ConfirmUnDelete});
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> UnDelete(string undeleteAction = GridParameters.UnDeleteMethod)
        {
            var routeValues = Context.GetRouteValueDictionary(undeleteAction, false);
            var command = new GridActionCommand<T>(Context, GridClass.UnDeleteIcon, GridParameters.UnDelete, routeValues, new {@class = GridClass.ConfirmUnDelete});
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> Delete<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridActionCommand<T>(Context, GridClass.DeleteIcon, GridParameters.Delete, routeValues, null);
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> ConfirmDelete<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridActionCommand<T>(Context, GridClass.DeleteIcon, GridParameters.Delete, routeValues, new {@class = GridClass.ConfirmDelete});
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        /// <summary>
        /// Delete command with a "confirm-delete" css class, used with an ajax confirm post action
        /// To use when the action to call is called "Delete" in the current controller.
        /// </summary>
        /// <returns></returns>
        public GridActionCommandBuilder<T> ConfirmDelete(string deleteAction = GridParameters.DeleteMethod)
        {
            var routeValues = Context.GetRouteValueDictionary(deleteAction, true);
            var command = new GridActionCommand<T>(Context, GridClass.DeleteIcon, GridParameters.Delete, routeValues, new {@class = GridClass.ConfirmDelete});
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> ConfirmDeleteMessage(string confirmDeleteMessageAction = GridParameters.ConfirmDeleteMessageMethod)
        {
            var routeValues = Context.GetRouteValueDictionary(confirmDeleteMessageAction, true);
            var command = new GridActionCommand<T>(Context, GridClass.DeleteIcon, GridParameters.Delete, routeValues, new {@class = GridClass.ConfirmDeleteAjax});
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> ConfirmDeleteMessage<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridActionCommand<T>(Context, GridClass.DeleteIcon, GridParameters.Delete, routeValues, new {@class = GridClass.ConfirmDeleteAjax});
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> ConfirmDeleteMessageForm(string confirmDeleteMessageAction = GridParameters.ConfirmDeleteMessageMethod)
        {
            var routeValues = Context.GetRouteValueDictionary(confirmDeleteMessageAction, true);
            var command = new GridActionCommand<T>(Context, GridClass.DeleteIcon, GridParameters.Delete, routeValues, new {@class = GridClass.ConfirmDeleteAjaxForm});
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> ConfirmDeleteMessageForm<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridActionCommand<T>(Context, GridClass.DeleteIcon, GridParameters.Delete, routeValues, new {@class = GridClass.ConfirmDeleteAjaxForm});
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> Detail<TController>(Expression<Action<TController>> actionResult, string alternateText = null) where TController : Controller
        {
            var alt = alternateText ?? GridParameters.Details;
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridActionCommand<T>(Context, GridClass.DetailIcon, alt, routeValues, null);
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> Validate<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridActionCommand<T>(Context, GridClass.ValidateIcon, GridParameters.Validate, routeValues, null);
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> Clone<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridActionCommand<T>(Context, GridClass.CloneIcon, GridParameters.Clone, routeValues, null);
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> Reinitialize<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridActionCommand<T>(Context, GridClass.ReinitializeIcon, GridParameters.Reinitilalize, routeValues, new {@class = GridClass.ConfirmAjaxReinitialize});
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> Geo<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridActionCommand<T>(Context, GridClass.GeoIcon, GridParameters.Geo, routeValues, null);
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> CheckGeo<TController>(Expression<Action<TController>> actionResult, Func<T, bool> hasValueFunc) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridCheckGeoActionCommand<T>(Context, GridClass.GeoIcon, GridParameters.Geo, routeValues, null, hasValueFunc);
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }

        public GridActionCommandBuilder<T> Custom<TController>(Expression<Action<TController>> actionResult, string cssClass, string altText) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridActionCommand<T>(Context, cssClass, altText, routeValues, null);
            Column.Commands.Add(command);
            return new GridActionCommandBuilder<T>(command);
        }
    }
}