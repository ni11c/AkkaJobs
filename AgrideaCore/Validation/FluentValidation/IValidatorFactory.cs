using Agridea.DataRepository;
using FluentValidation;

namespace Agridea.Validation.FluentValidation
{
    public interface IValidatorFactory<TPoco> where TPoco : PocoBase
    {
        AbstractValidator<TPoco> GetValidator(TPoco poco);
    }
}