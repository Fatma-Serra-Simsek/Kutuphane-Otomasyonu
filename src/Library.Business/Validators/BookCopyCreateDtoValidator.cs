using FluentValidation;
using Library.Business.Dtos;

namespace Library.Business.Validators;

public class BookCopyCreateDtoValidator : AbstractValidator<BookCopyCreateDto>
{
    public BookCopyCreateDtoValidator()
    {
        RuleFor(x => x.BookId).GreaterThan(0);
        RuleFor(x => x.Barcode).NotEmpty().MaximumLength(64);
    }
}
