using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Agridea.Resources;
using FluentValidation.Validators;

namespace Agridea.Validation.FluentValidation
{
    public class TotalIntegerValuesValidator : PropertyValidator
    {
        #region Constants
        private static readonly string Comma = ",";
        #endregion

        #region Members
        private IDictionary<Func<object, object>, MemberInfo> memberInfoForProperty_;
        #endregion

        #region Initialization
        public TotalIntegerValuesValidator(IDictionary<Func<object, object>, MemberInfo> memberInfoForProperty)
            : this(memberInfoForProperty, ValidationErrors.TotalIntegerValuesError)
        { }
        public TotalIntegerValuesValidator(IDictionary<Func<object, object>, MemberInfo> memberInfoForProperty, string errorMessage)
            : base(errorMessage)
        {
            memberInfoForProperty_ = memberInfoForProperty;
        }
        #endregion

        #region PropertyValidator
        protected override bool IsValid(PropertyValidatorContext context)
        {
            int expectedTotal = GetPropertiesTotal(context);
            if (Convert.ToInt32(context.PropertyValue) == expectedTotal) return true;

            context.MessageFormatter.AppendArgument(FluentValidationConstants.PropertyName, context.PropertyName.RemovePrefix());
            context.MessageFormatter.AppendArgument(FluentValidationConstants.PropertyValue, expectedTotal);
            context.MessageFormatter.AppendArgument(FluentValidationConstants.PropertyList, GetProperties());
            return false;
        }
        #endregion

        #region Helpers
        protected int GetPropertiesTotal(PropertyValidatorContext context)
        {
            return memberInfoForProperty_.Sum(x => Convert.ToInt32(x.Key(context.Instance)));
        }
        protected string GetProperties()
        {
            string properties = string.Empty;
            memberInfoForProperty_.ToList().ForEach(x =>
                properties += x.Value.Name.RemovePrefix() + Comma);
            return properties.Length > 0 ? properties.Substring(0, properties.Length - 1) : properties;
        }
        #endregion
    }
}
