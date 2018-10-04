using Agridea.Resources;
using FluentValidation;

namespace Agridea.Security
{
    public class LogOnDTOValidator : AbstractValidator<LogOnDTO>
    {
        public LogOnDTOValidator()
        {
            #region Basic validation
            RuleFor(model => model.UserName)
                .NotEmpty().WithMessage(AgrideaCoreStrings.CannotBeEmpty)
                .Length(3, 30).WithMessage(AgrideaCoreStrings.SecurityUserNameLength);
            RuleFor(model => model.Password)
                .NotEmpty().WithMessage(AgrideaCoreStrings.CannotBeEmpty)
                .Length(3, 30).WithMessage(AgrideaCoreStrings.SecurityPasswordLength);
            #endregion
        }
    }
}
