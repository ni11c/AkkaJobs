using System;
using Agridea.Resources;
using FluentValidation.Validators;

namespace Agridea.Validation.FluentValidation
{
    public class MinimumIntegerValueValidator : PropertyValidator
    {
        #region Members
        private int minValue_;
        #endregion

        #region Initialization
        public MinimumIntegerValueValidator(int minValue)
            : base(ValidationErrors.MinimumIntegerValueError)
        {
            minValue_ = minValue;
        }
        #endregion

        #region PropertyValidator
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (Convert.ToInt32(context.PropertyValue) >= minValue_) return true;

            context.MessageFormatter.AppendArgument(FluentValidationConstants.PropertyName, context.PropertyName.RemovePrefix());
            context.MessageFormatter.AppendArgument(FluentValidationConstants.Value, minValue_);
            return false;
        }
        #endregion
    }
}
