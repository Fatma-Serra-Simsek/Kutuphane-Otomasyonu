using FluentAssertions;
using Library.Business.Services;
using Library.Business.Validators;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Repositories;
using Moq;

namespace Library.Tests.Business;

public class BookServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IBookRepository> _books = new();
    private readonly Mock<IBookCopyRepository> _copies = new();
    private readonly Mock<ILoanRepository> _loans = new();
    private readonly BookService _sut;

    public BookServiceTests()
    {
        _uow.SetupGet(x => x.Books).Returns(_books.Object);
        _uow.SetupGet(x => x.BookCopies).Returns(_copies.Object);
        _uow.SetupGet(x => x.Loans).Returns(_loans.Object);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _sut = new BookService(
            _uow.Object,
            new BookCreateDtoValidator(),
            new BookUpdateDtoValidator(),
            new BookCopyCreateDtoValidator());
    }

    [Fact]
    public async Task SoftDelete_WithActiveLoan_Fails()
    {
        var book = new Book
        {
            Id = 1,
            Copies =
            {
                new BookCopy { Id = 2, Status = CopyStatus.Loaned, IsDeleted = false }
            }
        };
        _books.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(book);

        var result = await _sut.SoftDeleteAsync(1);

        result.Succeeded.Should().BeFalse();
        result.Error.Should().Contain("Ödünçteki");
    }

    [Fact]
    public async Task ChangeCopyStatus_AvailableToDamaged_Succeeds()
    {
        var copy = new BookCopy { Id = 1, Status = CopyStatus.Available };
        _copies.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(copy);

        var result = await _sut.ChangeCopyStatusAsync(1, CopyStatus.Damaged);

        result.Succeeded.Should().BeTrue();
        copy.Status.Should().Be(CopyStatus.Damaged);
    }

    [Fact]
    public async Task ChangeCopyStatus_AvailableToLoanedViaManual_Succeeds()
    {
        var copy = new BookCopy { Id = 1, Status = CopyStatus.Available };
        _copies.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(copy);

        var result = await _sut.ChangeCopyStatusAsync(1, CopyStatus.Loaned);

        result.Succeeded.Should().BeTrue();
        copy.Status.Should().Be(CopyStatus.Loaned);
    }

    [Fact]
    public async Task ChangeCopyStatus_LoanedToDamaged_Fails()
    {
        var copy = new BookCopy { Id = 1, Status = CopyStatus.Loaned };
        _copies.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(copy);

        var result = await _sut.ChangeCopyStatusAsync(1, CopyStatus.Damaged);

        result.Succeeded.Should().BeFalse();
        result.Error.Should().Contain("geçiş");
    }
}
