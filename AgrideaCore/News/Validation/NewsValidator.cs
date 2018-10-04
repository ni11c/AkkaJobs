using Agridea.Resources;
using FluentValidation;

namespace Agridea.News
{

    public class NewsValidator : AbstractValidator<NewsItem>
    {
        public NewsValidator()
        {
            #region Basic validation
            RuleFor(model => model.Title).NotEmpty()
                .WithMessage(AgrideaCoreStrings.CannotBeEmpty);

            RuleFor(model => model.Description).NotEmpty()
                .WithMessage(AgrideaCoreStrings.CannotBeEmpty);


            RuleFor(model => model.ValidityDateEnd)
                .Must((x, y) => (x.ValidityDateEnd >= x.ValidityDateStart))
                .When(x => x.ValidityDateStart != null && x.ValidityDateEnd != null)
                .WithMessage(AgrideaCoreStrings.NewsStartDateBeforEndDate);

            RuleFor(model => model.LinkUrl)
                .Matches("((https?):((//)|(\\\\))[\\w\\d:#%/;$()~_?\\-=\\\\.&]*)")
                .When(x => x.LinkUrl != null && x.LinkUrl != "")
                .WithMessage(AgrideaCoreStrings.NewsUrlSyntax);
            #endregion
        }
    }
}
