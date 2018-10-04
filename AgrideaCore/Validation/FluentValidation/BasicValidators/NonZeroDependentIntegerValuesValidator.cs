﻿using System;
using System.Reflection;
using Agridea.Resources;
using FluentValidation.Validators;

namespace Agridea.Validation.FluentValidation
{
    public class NonZeroDependentIntegerValuesValidator : PropertyValidator
    {
        #region Members
        private readonly Func<object, object> propertySelector_;
        private readonly MemberInfo memberToCompare_;
        #endregion

        #region Initialization
        public NonZeroDependentIntegerValuesValidator(Func<object, object> propertySelector, MemberInfo memberToCompare)
            : base(ValidationErrors.NonZeroDependentIntegerValuesError)
        {
            propertySelector_ = propertySelector;
            memberToCompare_ = memberToCompare;
        }
        #endregion

        #region PropertyValidator
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if ((Convert.ToInt32(GetComparisonValue(context)) != 0)
                .Implies(Convert.ToInt32(context.PropertyValue) != 0)) return true;

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
