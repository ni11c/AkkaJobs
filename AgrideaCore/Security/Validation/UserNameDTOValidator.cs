using Agridea.Resources;
using FluentValidation;

namespace Agridea.Security
{
    public class UserNameDTOValidator : AbstractValidator<UserNameDTO>
    {
        public UserNameDTOValidator()
        {
            #region Basic validation
            RuleFor(model => model.UserName)
                .NotEmpty().WithMessage(AgrideaCoreStrings.CannotBeEmpty)
                .Length(3, 30).WithMessage(AgrideaCoreStrings.SecurityUserNameLength);
            #endregion
        }
    }
}
