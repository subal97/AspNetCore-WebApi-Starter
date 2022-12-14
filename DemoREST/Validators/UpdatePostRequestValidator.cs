using DemoREST.Contracts.V1.Requests;
using FluentValidation;

namespace DemoREST.Validators
{
    public class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequest>
    {
        public UpdatePostRequestValidator()
        {
            RuleForEach(x => x.Tags).SetValidator(new TagRequestValidator());
        }
    }
}
