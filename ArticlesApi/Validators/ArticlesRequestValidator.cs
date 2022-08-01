using ArticlesApi.Dto;
using FluentValidation;

namespace ArticlesApi.Validators
{
    public class ArticlesRequestValidator: AbstractValidator<ArticlesRequest>
    {
        public ArticlesRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Price).NotEmpty();
            RuleFor(x => x.Stock).NotEmpty();
        }
    }
}
