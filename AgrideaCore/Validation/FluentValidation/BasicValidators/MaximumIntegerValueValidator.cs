using System;
using Agridea.Resources;
using FluentValidation.Validators;

namespace Agridea.Validation.FluentValidation
{
    public class MaximumIntegerValueValidator : PropertyValidator
    {
        #region Members
        private int maxValue_;
        #endregion

        #region Initialization
        public MaximumIntegerValueValidator(int maxValue)
            : base(ValidationErrors.MaximumIntegerValueError)
        {
            maxValue_ = maxValue;
        }
        #endregion

        #region PropertyValidator
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (Convert.ToInt32(context.PropertyValue) <= maxValue_) return true;

            context.MessageFormatter.AppendArgument(FluentValidationConstants.PropertyName, context.PropertyName.RemovePrefix());
            context.MessageFormatter.AppendArgument(FluentValidationConstants.Value, maxValue_);
            return false;
        }
        #endregion
    }
}
