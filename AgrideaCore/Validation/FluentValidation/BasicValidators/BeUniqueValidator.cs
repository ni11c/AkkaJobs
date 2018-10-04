using Agridea.DataRepository;
using Agridea.Resources;
using Agridea.Service;
using FluentValidation.Validators;
using System;
using System.Linq.Expressions;

namespace Agridea.Validation.FluentValidation
{
    public class BeUniqueValidator<T> : PropertyValidator where T : PocoBase
    {
        private readonly IService service_;


        public BeUniqueValidator(IService service)
            : base(ValidationErrors.BeUniqueError)
        {
            service_ = service;
        }


        protected override bool IsValid(PropertyValidatorContext context)
        {
            var expression = BuildExpression(context);
            return service_.IsUniqueFor(expression, context.Instance as T);
        }

        private static Expression<Func<T, bool>> BuildExpression(PropertyValidatorContext context)
        {
            var propertyValue = context.PropertyValue;
            if (context.PropertyName.EndsWith(".Id"))
            {
                propertyValue = propertyValue == null ? 0 : (int)propertyValue.GetType().GetProperty("Id").GetValue(propertyValue);
            }


            var actualType = typeof (T);
            var parameter = Expression.Parameter(actualType, "m");
            Expression left = parameter;
            foreach (var property in context.PropertyName.Split('.'))
            {
                var propertyInfo = actualType.GetProperty(property);
                left = Expression.Property(left, propertyInfo);
                actualType = propertyInfo.PropertyType;
            }
            Expression right = Expression.Constant(propertyValue ?? GetDefaultValue(actualType), actualType);
            return Expression.Lambda<Func<T, bool>>(Expression.Equal(left, right), new[] {parameter});
        }

        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
