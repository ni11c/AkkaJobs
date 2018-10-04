using System;
using System.Reflection;
using Agridea.Resources;
using FluentValidation.Validators;

namespace Agridea.Validation.FluentValidation
{
    public class NotZeroIfNotNullValidator : PropertyValidator
    {
        #region Members
        private readonly Func<object, object> propertySelector_;
        private readonly MemberInfo memberToCompare_;
        #endregion

        #region Initialization
        public NotZeroIfNotNullValidator(Func<object, object> propertySelector, MemberInfo memberToCompare)
            : base(ValidationErrors.NotZeroIfNotNullError)
        {
            propertySelector_ = propertySelector;
            memberToCompare_ = memberToCompare;
        }
        #endregion

        #region PropertyValidator
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if ((Convert.ToInt32(context.PropertyValue) == 0).Implies(GetComparisonValue(context) == null)) return true;

            context.MessageFormatter.AppendArgument(FluentValidationConstants.PropertyName, context.PropertyName.RemovePrefix());
            context.MessageFormatter.AppendArgument(FluentValidationConstants.PropertyToCompare, memberToCompare_.Name.RemovePrefix());
            return false;
        }
        #endregion

        #region Helpers
        private object GetComparisonValue(PropertyValidatorContext context)
        {
            if (propertySelector_ == null) return null;
            return propertySelector_(context.Instance);
        }
        #endregion
    }
}
