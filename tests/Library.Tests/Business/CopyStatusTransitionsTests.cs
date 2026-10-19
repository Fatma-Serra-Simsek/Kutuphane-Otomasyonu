using FluentAssertions;
using Library.Business.Services;
using Library.Domain.Enums;

namespace Library.Tests.Business;

public class CopyStatusTransitionsTests
{
    [Theory]
    [InlineData(CopyStatus.Available, CopyStatus.Damaged, true)]
    [InlineData(CopyStatus.Lost, CopyStatus.Available, true)]
    [InlineData(CopyStatus.Loaned, CopyStatus.Damaged, false)]
    public void CanTransition_MatchesMatrix(CopyStatus from, CopyStatus to, bool expected)
    {
        CopyStatusTransitions.CanTransition(from, to).Should().Be(expected);
    }
}
