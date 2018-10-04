using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace Agridea.Validation.FluentValidation
{
    public abstract class CompositeValidator<T> : AbstractValidator<T>
    {
        private readonly List<ConditionalValidator> otherValidators_ = new List<ConditionalValidator>();

        protected void ImportRulesFrom<TBase>(IValidator<TBase> validator, Func<T, bool> condition = null)
        {
            // Ensure that we've registered a compatible validator. 
            if (validator.CanValidateInstancesOfType(typeof(T)))
                otherValidators_.Add(new ConditionalValidator(validator, condition ?? (m => true)));

            else
                throw new NotSupportedException(string.Format("Type {0} is not a base-class or interface implemented by {1}.", typeof(TBase).Name, typeof(T).Name));

        }

        public override ValidationResult Validate(ValidationContext<T> context)
        {
            var mainErrors = base.Validate(context).Errors;
            var errorsFromOtherValidators = otherValidators_
                .Where(x => x.Condition(context.InstanceToValidate))
                .SelectMany(x => x.Validator.Validate(context).Errors);
            var combinedErrors = mainErrors.Concat(errorsFromOtherValidators);

            return new ValidationResult(combinedErrors);
        }
        private class ConditionalValidator
        {
            public ConditionalValidator(IValidator validator, Func<T, bool> condition)
            {
                Validator = validator;
                Condition = condition ?? (m => true);
            }

            public IValidator Validator { get; private set; }
            public Func<T, bool> Condition { get; private set; }
        }
    }

    


}