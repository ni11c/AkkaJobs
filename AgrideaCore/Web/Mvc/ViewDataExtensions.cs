using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Agridea.Web.Mvc
{
    public static class ViewDataExtensions
    {
        /// <summary>
        /// Drop this line "var errors = ViewData.GetModelStateErrors()" into your action code to debug weird validation message.
        /// Thanks to http://stackoverflow.com/questions/1461283/asp-net-mvc-updatemodel-not-updating-but-not-throwing-error
        /// </summary>
        public static IDictionary<string, string> GetModelStateErrors(this ViewDataDictionary viewDataDictionary)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var modelStateKey in viewDataDictionary.ModelState.Keys)
            {
                var modelStateValue = viewDataDictionary.ModelState[modelStateKey];
                foreach (var error in modelStateValue.Errors)
                {
                    var errorMessage = error.ErrorMessage;
                    var exception = error.Exception;
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        dict.Add(modelStateKey, "Egads! A Model Error Message! " + errorMessage);
                    }
                    if (exception != null)
                    {
                        dict.Add(modelStateKey, "Egads! A Model Error Exception! " + exception.ToString());
                    }
                }
            }
            return dict;
        }

        public static bool HasModelStateErrors(this ViewDataDictionary viewDataDictionary)
        {
            return viewDataDictionary.ModelState.Keys.Any(x => viewDataDictionary.ModelState[x].Errors.Any());
        }
    }
}
