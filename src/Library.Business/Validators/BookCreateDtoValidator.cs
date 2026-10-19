using FluentValidation;
using Library.Business.Dtos;

namespace Library.Business.Validators;

public class BookCreateDtoValidator : AbstractValidator<BookCreateDto>
{
    public BookCreateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ISBN).NotEmpty().MaximumLength(32);
        RuleFor(x => x.PublishYear).InclusiveBetween(1000, DateTime.Today.Year + 1);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.PublisherId).GreaterThan(0);
    }
}
