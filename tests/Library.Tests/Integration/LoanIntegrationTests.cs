using FluentAssertions;
using Library.Business.Results;
using Library.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Library.Tests.Integration;

[Collection("SqlServer")]
public class LoanIntegrationTests
{
    private readonly SqlServerFixture _fx;

    public LoanIntegrationTests(SqlServerFixture fx) => _fx = fx;

    [Fact]
    public async Task Borrow_ActiveMember_AvailableCopy_CreatesLoanAndSetsLoaned()
    {
        var ids = await _fx.SeedBorrowScenarioAsync();

        using var scope = _fx.CreateScope();
        var result = await _fx.GetLoanService(scope).BorrowBookAsync(ids.MemberId, ids.CopyId, ids.StaffId);

        result.IsSuccess.Should().BeTrue();
        result.Loan.Should().NotBeNull();
        result.Loan!.DueDate.Should().Be(DateTime.Today.AddDays(14));

        using var verify = _fx.CreateScope();
        var db = _fx.GetDb(verify);
        var copy = await db.BookCopies.SingleAsync(c => c.Id == ids.CopyId);
        copy.Status.Should().Be(CopyStatus.Loaned);
        (await db.Loans.CountAsync(l => l.BookCopyId == ids.CopyId && l.ReturnDate == null)).Should().Be(1);
    }

    [Fact]
    public async Task Borrow_InactiveMember_ReturnsMemberInactive()
    {
        var ids = await _fx.SeedBorrowScenarioAsync(memberActive: false);

        using var scope = _fx.CreateScope();
        var result = await _fx.GetLoanService(scope).BorrowBookAsync(ids.MemberId, ids.CopyId, ids.StaffId);

        result.Code.Should().Be(LoanResultCode.MemberInactive);
    }

    [Fact]
    public async Task Borrow_AlreadyLoanedCopy_ReturnsCopyNotAvailable()
    {
        var ids = await _fx.SeedBorrowScenarioAsync(copyStatus: CopyStatus.Loaned);

        using var scope = _fx.CreateScope();
        var result = await _fx.GetLoanService(scope).BorrowBookAsync(ids.MemberId, ids.CopyId, ids.StaffId);

        result.Code.Should().Be(LoanResultCode.CopyNotAvailable);
    }

    [Fact]
    public async Task Return_OnTime_ClearsLoanAndSetsAvailable()
    {
        var ids = await _fx.SeedBorrowScenarioAsync();
        using (var scope = _fx.CreateScope())
        {
            (await _fx.GetLoanService(scope).BorrowBookAsync(ids.MemberId, ids.CopyId, ids.StaffId))
                .IsSuccess.Should().BeTrue();
        }

        using var retScope = _fx.CreateScope();
        var ret = await _fx.GetLoanService(retScope).ReturnBookAsync(ids.CopyId);

        ret.IsSuccess.Should().BeTrue();
        ret.DaysOverdue.Should().Be(0);

        using var verify = _fx.CreateScope();
        var db = _fx.GetDb(verify);
        var copy = await db.BookCopies.SingleAsync(c => c.Id == ids.CopyId);
        copy.Status.Should().Be(CopyStatus.Available);
        (await db.Loans.CountAsync(l => l.BookCopyId == ids.CopyId && l.ReturnDate == null)).Should().Be(0);
    }

    [Fact]
    public async Task Return_OverdueLoan_ReportsDaysOverdue()
    {
        var ids = await _fx.SeedBorrowScenarioAsync();
        using (var scope = _fx.CreateScope())
        {
            (await _fx.GetLoanService(scope).BorrowBookAsync(ids.MemberId, ids.CopyId, ids.StaffId))
                .IsSuccess.Should().BeTrue();
        }

        using (var fix = _fx.CreateScope())
        {
            var db = _fx.GetDb(fix);
            var loan = await db.Loans.SingleAsync(l => l.BookCopyId == ids.CopyId && l.ReturnDate == null);
            loan.DueDate = DateTime.Today.AddDays(-3);
            await db.SaveChangesAsync();
        }

        using var retScope = _fx.CreateScope();
        var ret = await _fx.GetLoanService(retScope).ReturnBookAsync(ids.CopyId);

        ret.IsSuccess.Should().BeTrue();
        ret.DaysOverdue.Should().Be(3);
    }

    [Fact]
    public async Task Return_NoActiveLoan_FailsCleanly()
    {
        var ids = await _fx.SeedBorrowScenarioAsync();

        using var scope = _fx.CreateScope();
        var ret = await _fx.GetLoanService(scope).ReturnBookAsync(ids.CopyId);

        ret.Code.Should().Be(LoanResultCode.NoActiveLoan);
    }
}
