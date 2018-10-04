using Agridea.Validation.FluentValidation;
using FluentValidation;

namespace Agridea.Security
{
    public static class FluentValidatorExtensions
    {
        #region IRuleBuilders
        public static IRuleBuilderOptions<T, int> ThisIsJustAnExample<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new MaximumIntegerValueValidator(200));
        }
        #endregion
    }
}
