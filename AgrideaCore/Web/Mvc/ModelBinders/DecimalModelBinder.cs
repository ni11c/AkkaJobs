using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Agridea.Web.Mvc
{
    /// <summary>
    /// Thanks http://haacked.com/archive/2011/03/19/fixing-binding-to-decimals.aspx/
    /// </summary>
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            decimal actualValue = 0M;
            bool couldConvert = false;
            var cultures = new[]
            {
                CultureInfo.CurrentCulture, 
                CultureInfo.CreateSpecificCulture("en-US")
            };
            foreach (var culture in cultures)
            {
                couldConvert = Decimal.TryParse(valueResult.AttemptedValue, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, culture, out actualValue);
                if (couldConvert)
                    break;
            }
            
            if (!couldConvert)
                modelState.Errors.Add(new FormatException(string.Format("could not convert string {0} to decimal using cultures {1}", valueResult, string.Join(", ", cultures.Select(c => c.Name)))));

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}
