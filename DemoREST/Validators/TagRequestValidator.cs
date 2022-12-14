using DemoREST.Contracts.V1.Requests;
using FluentValidation;

namespace DemoREST.Validators
{
    public class TagRequestValidator : AbstractValidator<TagRequest>
    {
        public TagRequestValidator()
        {
            //allow only non empty alpha numeric characters
            RuleFor(x => x.TagName)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$");
        }
    }
}
