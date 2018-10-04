using Agridea.Web.Mvc.Grid.Command;
using Agridea.Web.Mvc.Grid.ToolBar;
using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public class GridToolBarCommandFactory<T> : IHideObjectMembers
    {
        public ControllerContext Context { get; private set; }
        public GridToolBar<T> ToolBar { get; private set; }
        public GridToolBarCommandFactory(GridToolBar<T> toolBar, ControllerContext context)
        {
            Context = context;
            ToolBar = toolBar;
        }

        public GridCreateActionCommandBuilder<T> Create<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridCreateActionCommand<T>(Context, GridClass.IconAdd, GridParameters.Create, routeValues,  null);
            ToolBar.Commands.Add(command);
            return new GridCreateActionCommandBuilder<T>(command);
        }

        public GridCreateActionCommandBuilder<T> Create(string createAction = GridParameters.CreateMethod)
        {
            var routeValues = Context.GetRouteValueDictionary(createAction, false);
            var command = new GridCreateActionCommand<T>(Context, GridClass.IconAdd, GridParameters.Create, routeValues, null);
            ToolBar.Commands.Add(command);
            return new GridCreateActionCommandBuilder<T>(command);
        }

        public GridExcelActionCommandBuilder<T> ExportToExcel<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridExcelActionCommand<T>(Context, GridClass.ExcelExport, GridParameters.ExportToExcel, routeValues, null);
            ToolBar.Commands.Add(command);
            return new GridExcelActionCommandBuilder<T>(command);
        }
        public GridExcelActionCommandBuilder<T> ExportToPdf<TController>(Expression<Action<TController>> actionResult) where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(actionResult);
            var command = new GridExcelActionCommand<T>(Context, GridClass.PdfExport, GridParameters.ExportToPdf, routeValues, null);
            ToolBar.Commands.Add(command);
            return new GridExcelActionCommandBuilder<T>(command);
        }
    }
}
