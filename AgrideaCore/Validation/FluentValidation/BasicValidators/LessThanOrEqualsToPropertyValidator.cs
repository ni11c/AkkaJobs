using System;
using System.Reflection;
using Agridea.Resources;
using FluentValidation.Validators;

namespace Agridea.Validation.FluentValidation
{
    public class LessThanOrEqualsToPropertyValidator : PropertyValidator
    {
        #region Members
        private readonly Func<object, object> propertySelector_;
        private readonly MemberInfo memberToCompare_;
        #endregion

        #region Initialization
        public LessThanOrEqualsToPropertyValidator(Func<object, object> propertySelector, MemberInfo memberToCompare)
            : base(ValidationErrors.LessOrEqualsError)
        {
            propertySelector_ = propertySelector;
            memberToCompare_ = memberToCompare;
        }
        #endregion

        #region PropertyValidator
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (Convert.ToInt32(context.PropertyValue) <= GetComparisonValue(context)) return true;

            context.MessageFormatter.AppendArgument(FluentValidationConstants.PropertyName, context.PropertyName.RemovePrefix());
            context.MessageFormatter.AppendArgument(FluentValidationConstants.PropertyToCompare, memberToCompare_.Name.RemovePrefix());
            return false;
        }
        #endregion

        #region Helpers
        private int GetComparisonValue(PropertyValidatorContext context)
        {
            return Convert.ToInt32(propertySelector_(context.Instance));
        }
        #endregion
    }
}
