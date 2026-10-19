using FluentAssertions;
using Library.Business.Results;
using Library.Business.Services;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Repositories;
using Moq;

namespace Library.Tests.Business;

public class LoanServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMemberRepository> _members = new();
    private readonly Mock<IBookCopyRepository> _copies = new();
    private readonly Mock<ILoanRepository> _loans = new();
    private readonly Mock<ISettingRepository> _settings = new();
    private readonly Mock<IUnitOfWorkTransaction> _tx = new();
    private readonly LoanService _sut;

    public LoanServiceTests()
    {
        _uow.SetupGet(x => x.Members).Returns(_members.Object);
        _uow.SetupGet(x => x.BookCopies).Returns(_copies.Object);
        _uow.SetupGet(x => x.Loans).Returns(_loans.Object);
        _uow.SetupGet(x => x.Settings).Returns(_settings.Object);
        _uow.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_tx.Object);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _tx.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _tx.Setup(x => x.DisposeAsync()).Returns(ValueTask.CompletedTask);

        _settings.Setup(x => x.GetValueAsync(LoanService.LoanPeriodDaysKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync("14");

        _sut = new LoanService(_uow.Object);
    }

    [Fact]
    public async Task BorrowBook_InactiveMember_ReturnsMemberInactive()
    {
        _members.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Member { Id = 1, IsActive = false });

        var result = await _sut.BorrowBookAsync(1, 10, 5);

        result.Code.Should().Be(LoanResultCode.MemberInactive);
    }

    [Fact]
    public async Task BorrowBook_CopyNotAvailable_ReturnsCopyNotAvailable()
    {
        _members.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Member { Id = 1, IsActive = true });
        _copies.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookCopy { Id = 10, Status = CopyStatus.Loaned });

        var result = await _sut.BorrowBookAsync(1, 10, 5);

        result.Code.Should().Be(LoanResultCode.CopyNotAvailable);
    }

    [Fact]
    public async Task BorrowBook_MissingCopy_ReturnsCopyNotFound()
    {
        _members.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Member { Id = 1, IsActive = true });
        _copies.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookCopy?)null);

        var result = await _sut.BorrowBookAsync(1, 10, 5);

        result.Code.Should().Be(LoanResultCode.CopyNotFound);
    }

    [Fact]
    public async Task BorrowBook_Success_SetsDueDateFromSettings()
    {
        _members.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Member { Id = 1, IsActive = true });
        var copy = new BookCopy { Id = 10, Status = CopyStatus.Available };
        _copies.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(copy);
        _loans.Setup(x => x.GetActiveByCopyAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Loan?)null);
        _settings.Setup(x => x.GetValueAsync(LoanService.LoanPeriodDaysKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync("7");

        Loan? added = null;
        _loans.Setup(x => x.Add(It.IsAny<Loan>())).Callback<Loan>(l => added = l);

        var result = await _sut.BorrowBookAsync(1, 10, 5);

        result.IsSuccess.Should().BeTrue();
        added.Should().NotBeNull();
        added!.LoanDate.Should().Be(DateTime.Today);
        added.DueDate.Should().Be(DateTime.Today.AddDays(7));
        copy.Status.Should().Be(CopyStatus.Loaned);
        _tx.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BorrowBook_ActiveLoanExists_ReturnsCopyNotAvailable()
    {
        _members.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Member { Id = 1, IsActive = true });
        _copies.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookCopy { Id = 10, Status = CopyStatus.Available });
        _loans.Setup(x => x.GetActiveByCopyAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Loan { Id = 99, BookCopyId = 10, ReturnDate = null });

        var result = await _sut.BorrowBookAsync(1, 10, 5);

        result.Code.Should().Be(LoanResultCode.CopyNotAvailable);
    }

    [Fact]
    public async Task ReturnBook_NoActiveLoan_ReturnsNoActiveLoan()
    {
        _loans.Setup(x => x.GetActiveByCopyAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Loan?)null);

        var result = await _sut.ReturnBookAsync(10);

        result.Code.Should().Be(LoanResultCode.NoActiveLoan);
    }

    [Fact]
    public async Task ReturnBook_Overdue_ReturnsDaysOverdue()
    {
        var loan = new Loan
        {
            Id = 1,
            BookCopyId = 10,
            LoanDate = DateTime.Today.AddDays(-20),
            DueDate = DateTime.Today.AddDays(-5),
            ReturnDate = null
        };
        var copy = new BookCopy { Id = 10, Status = CopyStatus.Loaned };

        _loans.Setup(x => x.GetActiveByCopyAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loan);
        _copies.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(copy);

        var result = await _sut.ReturnBookAsync(10);

        result.IsSuccess.Should().BeTrue();
        result.DaysOverdue.Should().Be(5);
        loan.ReturnDate.Should().Be(DateTime.Today);
        copy.Status.Should().Be(CopyStatus.Available);
    }

    [Fact]
    public async Task BorrowBook_MemberNotFound_ReturnsMemberNotFound()
    {
        _members.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Member?)null);

        var result = await _sut.BorrowBookAsync(1, 10, 5);

        result.Code.Should().Be(LoanResultCode.MemberNotFound);
    }
}
