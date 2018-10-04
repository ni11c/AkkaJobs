using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Agridea.Web.Mvc.Grid;
using System.Collections.Generic;

namespace Agridea.Web.Mvc
{
    public static class GridHelper
    {
        public static Grid<T> Grid<T>(this HtmlHelper helper)
        {
            return new Grid<T>(helper, helper.ViewData.Model as IQueryable<T>);
        }
        public static Grid<T> Grid<T>(this HtmlHelper helper, IQueryable<T> model, string bindingListName = null)
        {
            return new Grid<T>(helper, model, bindingListName);
        }

        public static Grid<T> Grid<T>(this Controller controller, IQueryable<T> model)
        {
            return new Grid<T>(model, controller.ViewData, controller.ControllerContext, controller.TempData);
        }

        public static Grid<T> Grid<TModel, T>(this HtmlHelper helper, Func<TModel, IQueryable<T>> func) where TModel : class
        {
            var model = (func(helper.ViewData.Model as TModel));
            return new Grid<T>(helper, model);
        }
        public static Grid<T> Grid<TModel, T>(this HtmlHelper helper, Func<TModel, IQueryable<T>> func, Expression<Func<TModel, IList<T>>> bindingListExpression)
            where TModel : class
        {
            var bindingListName = ExpressionHelper.GetExpressionText(bindingListExpression);
            var model = (func(helper.ViewData.Model as TModel));
            return new Grid<T>(helper, model, bindingListName);
        }
    }
}
