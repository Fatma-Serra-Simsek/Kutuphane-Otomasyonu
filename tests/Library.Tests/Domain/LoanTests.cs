using FluentAssertions;
using Library.Domain.Entities;

namespace Library.Tests.Domain;

public class LoanTests
{
    [Fact]
    public void IsOverdue_ReturnsTrue_WhenNotReturnedAndDueDateBeforeToday()
    {
        var loan = new Loan
        {
            DueDate = DateTime.Today.AddDays(-1),
            ReturnDate = null
        };

        loan.IsOverdue().Should().BeTrue();
    }

    [Fact]
    public void IsOverdue_ReturnsFalse_WhenReturned()
    {
        var loan = new Loan
        {
            DueDate = DateTime.Today.AddDays(-5),
            ReturnDate = DateTime.Today
        };

        loan.IsOverdue().Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_ReturnsFalse_WhenDueDateIsToday()
    {
        var loan = new Loan
        {
            DueDate = DateTime.Today,
            ReturnDate = null
        };

        loan.IsOverdue().Should().BeFalse();
    }
}
