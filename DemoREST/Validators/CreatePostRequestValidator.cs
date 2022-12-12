using DemoREST.Contracts.V1.Requests;
using FluentValidation;

namespace DemoREST.Validators
{
    public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
    {
        public CreatePostRequestValidator()
        {
            RuleForEach(x => x.Tags).SetValidator(new TagRequestValidator());
        }
    }
}
