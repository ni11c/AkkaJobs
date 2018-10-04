using Agridea.Resources;
using FluentValidation;

namespace Agridea.Security
{
    public class RoleNameDTOValidator : AbstractValidator<RoleNameDTO>
    {
        public RoleNameDTOValidator()
        {
            #region Basic validation
            RuleFor(model => model.Name)
                .NotEmpty().WithMessage(AgrideaCoreStrings.CannotBeEmpty)
                .Length(3, 30).WithMessage(AgrideaCoreStrings.SecurityRoleNameLength);
            #endregion
        }
    }
}
