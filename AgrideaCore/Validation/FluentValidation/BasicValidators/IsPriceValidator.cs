using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agridea.Resources;
using FluentValidation.Validators;

namespace Agridea.Validation.FluentValidation
{
    public class IsPriceValidator : PropertyValidator
    {
        #region Initialization
        public IsPriceValidator() : base(ValidationErrors.PriceError)
        { }
        #endregion

        #region PropertyValidator
        protected override bool IsValid(PropertyValidatorContext context)
        {
            
            if (Convert.ToDecimal(context.PropertyValue) >= 0) return true;

            context.MessageFormatter.AppendArgument(FluentValidationConstants.PropertyName, context.PropertyName.RemovePrefix());
            return false;
        }
        #endregion
    }
}
