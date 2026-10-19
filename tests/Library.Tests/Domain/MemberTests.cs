using FluentAssertions;
using Library.Domain.Entities;

namespace Library.Tests.Domain;

public class MemberTests
{
    [Fact]
    public void CanBorrow_ReturnsTrue_WhenActive()
    {
        var member = new Member { IsActive = true };
        member.CanBorrow().Should().BeTrue();
    }

    [Fact]
    public void CanBorrow_ReturnsFalse_WhenInactive()
    {
        var member = new Member { IsActive = false };
        member.CanBorrow().Should().BeFalse();
    }
}
