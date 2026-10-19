using FluentAssertions;
using Library.Business.Results;
using Library.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Library.Tests.Integration;

[Collection("SqlServer")]
public class LoanConcurrencyTests
{
    private readonly SqlServerFixture _fx;

    public LoanConcurrencyTests(SqlServerFixture fx) => _fx = fx;

    [Fact]
    public async Task ConcurrentBorrow_SameCopy_OnlyOneSucceeds()
    {
        var first = await _fx.SeedBorrowScenarioAsync();
        var secondMember = await _fx.SeedBorrowScenarioAsync();

        // İkinci üye, aynı kopyaya yarışacak; kendi kopyasını kullanma.
        using (var prep = _fx.CreateScope())
        {
            var db = _fx.GetDb(prep);
            var unused = await db.BookCopies.SingleAsync(c => c.Id == secondMember.CopyId);
            db.BookCopies.Remove(unused);
            await db.SaveChangesAsync();
        }

        using var scopeA = _fx.CreateScope();
        using var scopeB = _fx.CreateScope();
        var loanA = _fx.GetLoanService(scopeA);
        var loanB = _fx.GetLoanService(scopeB);

        var taskA = loanA.BorrowBookAsync(first.MemberId, first.CopyId, first.StaffId);
        var taskB = loanB.BorrowBookAsync(secondMember.MemberId, first.CopyId, first.StaffId);
        await Task.WhenAll(taskA, taskB);

        var results = new[] { await taskA, await taskB };
        results.Count(r => r.IsSuccess).Should().Be(1);
        results.Count(r => r.Code == LoanResultCode.CopyNotAvailable).Should().Be(1);

        using var verify = _fx.CreateScope();
        var dbVerify = _fx.GetDb(verify);
        var active = await dbVerify.Loans.CountAsync(l => l.BookCopyId == first.CopyId && l.ReturnDate == null);
        active.Should().Be(1);

        var copy = await dbVerify.BookCopies.SingleAsync(c => c.Id == first.CopyId);
        copy.Status.Should().Be(CopyStatus.Loaned);
    }
}
