using FluentValidation;
using Library.Business.Dtos;

namespace Library.Business.Validators;

public class MemberUpdateDtoValidator : AbstractValidator<MemberUpdateDto>
{
    public MemberUpdateDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.MemberNumber).NotEmpty().MaximumLength(32);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Phone).MaximumLength(32);
    }
}
