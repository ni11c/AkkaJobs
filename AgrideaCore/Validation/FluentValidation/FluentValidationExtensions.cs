using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;
using Agridea.ObjectMapping;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Agridea.Service;
using Agridea.DataRepository;
namespace Agridea.Validation.FluentValidation
{
    public static class FluentValidatorExtensions
    {
        #region IRuleBuilders
        public static IRuleBuilderOptions<T, int> IsPositiveOrZero<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MinimumIntegerValueValidator(0));
        }
        public static IRuleBuilderOptions<T, int> IsPositive<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MinimumIntegerValueValidator(1));
        }
        public static IRuleBuilderOptions<T, TProperty> LessThanOrEqualsProperty<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<T, TProperty>> expression)
        {
            var func = expression.Compile();
            return ruleBuilder.SetValidator(new LessThanOrEqualsToPropertyValidator(func.CoerceToNonGeneric(), expression.GetMember()));
        }

        /* PBE for .Compile() removing
        public static IRuleBuilderOptions<T, TProperty> IsTotalOf<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, params Expression<Func<T, TProperty>>[] list)
        {
            IDictionary<Func<object, object>, MemberInfo> dictionary = new Dictionary<Func<object, object>, MemberInfo>();
            foreach (Expression<Func<T, TProperty>> expression in list)
            {
                var func = expression.Compile();
                dictionary.Add(func.CoerceToNonGeneric(), expression.GetMember());
            }
            return ruleBuilder.SetValidator(new TotalIntegerValuesValidator(dictionary));
        }
        */

        public static IRuleBuilderOptions<T, decimal> IsPrice<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IsPriceValidator());
        }

        public static IRuleBuilderOptions<T, TProperty> IsUnique<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, IService service)
            where T : PocoBase
        {
            return ruleBuilder.SetValidator(new BeUniqueValidator<T>(service));
        }
        #endregion

        #region Exception Extensions
        public static string Format(this ValidationFailure error)
        {
            return error.ErrorMessage;
        }
        public static string Format(this ValidationFailure error, string message)
        {
            return string.Format("{0} : {1}", message, error.ErrorMessage);
        }
        public static ValidationException LogWith(this ValidationException exception)
        {
            foreach (var error in exception.Errors)
                Log.Error(error.Format());

            return exception;
        }
        public static ValidationException LogWith(this ValidationException exception, string message)
        {
            foreach (var error in exception.Errors)
                Log.Error(error.Format(message));

            return exception;
        }
        public static ValidationException CopyTo(this ValidationException exception, ModelStateDictionary modelState)
        {
            foreach (var error in exception.Errors)
                AddErrorToModelState(modelState, error.PropertyName, error.Format());

            return exception;
        }
        public static ValidationException CopyToWithIndex(this ValidationException exception, ModelStateDictionary modelState, int index)
        {
            foreach (var error in exception.Errors)
                AddErrorToModelState(modelState, string.Format("[{0}].{1}", index, error.PropertyName), error.Format());

            return exception;
        }
        public static ValidationException CopyToWithIndexAndMessage(this ValidationException exception, ModelStateDictionary modelState, int index, string message)
        {
            foreach (var error in exception.Errors)
                AddErrorToModelState(modelState, string.Format("[{0}].{1}", index, error.PropertyName), error.Format(message));

            return exception;
        }
        public static ValidationException CopyToWithIndexAndPrefix(this ValidationException exception, ModelStateDictionary modelState, int index, string prefix)
        {
            foreach (var error in exception.Errors)
                AddErrorToModelState(modelState, string.Format("{0}[{1}].{2}", prefix, index, error.PropertyName), error.Format());

            return exception;
        }
        public static ValidationException CopyToWithIndexPrefixMessage(this ValidationException exception, ModelStateDictionary modelState, string prefix, int index, string message)
        {
            foreach (var error in exception.Errors)
                AddErrorToModelState(modelState, string.Format("{0}[{1}].{2}", prefix, index, error.PropertyName), error.Format(message));

            return exception;
        }
        #endregion

        #region String Extensions
        public static string RemovePrefix(this string propertyName)
        {
            if (FluentValidationConstants.CurrentPrefix != null)
                propertyName = propertyName.Replace(FluentValidationConstants.CurrentPrefix, string.Empty);
            return propertyName
                .Replace(".", string.Empty);
        }
        #endregion

        #region IRuleBuilderOptions
        public static IRuleBuilderOptions<T, TProperty> UsePropertyId<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule)
        {
            Asserts<ArgumentException>.IsTrue(typeof(TProperty).IsClass, string.Format("{0} should be a class", typeof(TProperty).ToString()));
            Asserts<ArgumentException>.IsNotNull(typeof(TProperty).GetProperty("Id"), string.Format("{0} should have a property Id", typeof(TProperty).ToString()));
            return rule.Configure(config => config.PropertyName = config.PropertyName + ".Id");
        }

        public static IRuleBuilderOptions<T, TProperty> AsWarning<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule)
        {
            return rule.WithState<T, TProperty>(x => ValidationErrorLevel.Warning);
        }
        #endregion

        public static bool HasWarnings(this ValidationResult result)
        {
            return result.GetWarnings().Any();
        }
        public static bool HasErrors(this ValidationResult result)
        {
            return result.GetErrors().Any();
        }

        public static IList<ValidationFailure> GetWarnings(this ValidationResult result)
        {
            return result.Errors.Where(m => m.CustomState != null && Convert.ToInt32(m.CustomState) == (int)ValidationErrorLevel.Warning).ToList();
        }
        public static IList<ValidationFailure> GetErrors(this ValidationResult result)
        {
            return result.Errors.Except(result.GetWarnings()).ToList();
        }

        #region Helpers
        private static void AddErrorToModelState(ModelStateDictionary modelState, string propertyName, string message)
        {
            string actualPropertyName = propertyName;
            var prefix = string.Empty;
            var mapContext = EasyMapper.Context;
            if (mapContext.Exists)
            {
                var indexOfPrefix = propertyName.IndexOf(']');

                if (indexOfPrefix != -1)
                {
                    prefix = propertyName.Substring(0, indexOfPrefix + 2);
                    propertyName = propertyName.Substring(indexOfPrefix + 2);

                }
                var viewModelPropertyPath = EasyMapper.GetMap(mapContext.TargetType, mapContext.SourceType, propertyName);

                //In debug it will throw to enforce developper to look at the issue
                //When view model are used everywhere...
                //Asserts<InvalidOperationException>.IsNotNull(viewModelPropertyPath, string.Format("Property '{0}' is not correctly mapped", propertyName));

                //In release it will use the propertyname, the rendering will miss proper routing to the invalidated property but it wont crash
                if (viewModelPropertyPath != null) actualPropertyName = prefix + viewModelPropertyPath.Path;
            }

            modelState.AddModelError(actualPropertyName, message);
        }
        #endregion
    }

}
