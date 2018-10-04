using System;
using System.Collections.Generic;
using System.Reflection;
using Agridea.Resources;
using FluentValidation.Validators;

namespace Agridea.Validation.FluentValidation
{
    public class TotalMinimumIntegerValueValidator : TotalIntegerValuesValidator
    {
        #region Members
        private int minValue_;
        #endregion

        #region Initialization
        public TotalMinimumIntegerValueValidator(int minValue, IDictionary<Func<object, object>, MemberInfo> memberInfoForProperty)
            : base(memberInfoForProperty, ValidationErrors.TotalMinimumIntegerValuesError)
        {
            minValue_ = minValue;
        }
        #endregion

        #region PropertyValidator
        protected override bool IsValid(PropertyValidatorContext context)
        {
            int total = GetPropertiesTotal(context);
            if (total >= minValue_) return true;

            context.MessageFormatter.AppendArgument(FluentValidationConstants.PropertyList, GetProperties());
            context.MessageFormatter.AppendArgument(FluentValidationConstants.Value, minValue_);
            return false;
        }
        #endregion
    }
}
