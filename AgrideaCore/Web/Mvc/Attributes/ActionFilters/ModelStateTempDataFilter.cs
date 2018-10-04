using System.Web.Mvc;

namespace Agridea.Web.Mvc.ActionFilters
{
    public abstract class ModelStateTempDataFilter : ActionFilterAttribute
    {
        protected static readonly string Key = typeof(ModelStateTempDataFilter).FullName;
    }
}