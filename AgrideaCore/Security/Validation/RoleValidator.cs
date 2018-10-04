using Agridea.Resources;
using Agridea.Validation.FluentValidation;
using FluentValidation;

namespace Agridea.Security
{
    public class RoleValidator : AbstractValidator<Role>
    {
        private IAgrideaService service_;

        public RoleValidator(IAgrideaService service)
        {
            service_ = service;

            RuleFor(model => model.Name)
                .NotEmpty().WithMessage(AgrideaCoreStrings.CannotBeEmpty)
                .Length(3, 30).WithMessage(AgrideaCoreStrings.SecurityPasswordLength);

            RuleFor(model => model.Name)
                .IsUnique(service)
                .WithName("Nom du rôle");

            RuleFor(model => model.Name)
                .Must(name => service_.GetRoleByName(name) == null)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(AgrideaCoreStrings.SecurityRoleAlreadyExists);
        }
    }
}
