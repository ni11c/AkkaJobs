using Agridea.Resources;
using Agridea.Validation.FluentValidation;
using FluentValidation;

namespace Agridea.Security
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator(IAgrideaService service)
        {
            RuleFor(model => model.UserName)
                .NotEmpty().WithMessage(AgrideaCoreStrings.CannotBeEmpty)
                .Length(3, 30).WithMessage(AgrideaCoreStrings.SecurityPasswordLength);

            RuleFor(m => m.UserName)
                .IsUnique(service);

            RuleFor(model => model.Email)
                .EmailAddress().WithMessage(AgrideaCoreStrings.SecurityEmail);

        }
    }
}
