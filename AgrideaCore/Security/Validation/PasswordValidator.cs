using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agridea.Resources;
using FluentValidation;

namespace Agridea.Security
{
    public class PasswordValidator : AbstractValidator<PasswordEdit>
    {
        public PasswordValidator()
        {
            RuleFor(model => model.UserName)
                .NotEmpty().WithMessage(AgrideaCoreStrings.CannotBeEmpty)
                .Length(3, 30).WithMessage(AgrideaCoreStrings.SecurityPasswordLength);
            RuleFor(model => model.Password)
                .NotEmpty()
                .WithName("Le mot de passe");
            RuleFor(model => model.Password)
                .Length(3, 30).WithMessage(AgrideaCoreStrings.SecurityPasswordLength)
                .When(model => model.Password != null);
            RuleFor(model => model.ConfirmPassword)
                .Length(3, 30).WithMessage(AgrideaCoreStrings.SecurityPasswordLength)
                .When(model => model.Password != null)
                .Equal(model => model.Password).WithMessage(AgrideaCoreStrings.SecurityPasswordConfirmation);
        }
    }
}
