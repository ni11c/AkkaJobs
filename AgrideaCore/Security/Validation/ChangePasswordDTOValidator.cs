using Agridea.Resources;
using FluentValidation;

namespace Agridea.Security
{
    public class ChangePasswordDTOValidator : AbstractValidator<ChangePasswordDTO>
    {
        public ChangePasswordDTOValidator()
        {
            #region Basic validation
            RuleFor(model => model.OldPassword)
                .NotEmpty().WithMessage(AgrideaCoreStrings.CannotBeEmpty)
                .Length(3, 30).WithMessage(AgrideaCoreStrings.SecurityPasswordLength);
            RuleFor(model => model.NewPassword)
                 .NotEmpty().WithMessage(AgrideaCoreStrings.CannotBeEmpty)
                 .Length(3, 30).WithMessage(AgrideaCoreStrings.SecurityPasswordLength);
            RuleFor(model => model.ConfirmPassword)
                 .NotEmpty().WithMessage(AgrideaCoreStrings.CannotBeEmpty)
                 .Length(3, 30).WithMessage(AgrideaCoreStrings.SecurityPasswordLength)
                 .Equal(model => model.NewPassword).WithMessage(AgrideaCoreStrings.SecurityPasswordConfirmation);
            #endregion
        }
    }
}
