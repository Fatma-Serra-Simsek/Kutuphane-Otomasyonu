using FluentAssertions;
using Library.Business.Services;
using Library.Business.Validators;
using Library.Domain.Entities;
using Library.Domain.Repositories;
using Moq;

namespace Library.Tests.Business;

public class MemberServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMemberRepository> _members = new();
    private readonly Mock<ILoanRepository> _loans = new();
    private readonly MemberService _sut;

    public MemberServiceTests()
    {
        _uow.SetupGet(x => x.Members).Returns(_members.Object);
        _uow.SetupGet(x => x.Loans).Returns(_loans.Object);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _sut = new MemberService(
            _uow.Object,
            new MemberCreateDtoValidator(),
            new MemberUpdateDtoValidator());
    }

    [Fact]
    public async Task SoftDelete_WithActiveLoan_Fails()
    {
        _members.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Member { Id = 1, IsActive = true });
        _loans.Setup(x => x.GetByMemberAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Loan>
            {
                new() { Id = 9, MemberId = 1, ReturnDate = null }
            });

        var result = await _sut.SoftDeleteAsync(1);

        result.Succeeded.Should().BeFalse();
        result.Error.Should().Contain("Aktif ödüncü");
    }

    [Fact]
    public async Task SoftDelete_WithoutActiveLoan_Succeeds()
    {
        var member = new Member { Id = 1, IsActive = true };
        _members.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _loans.Setup(x => x.GetByMemberAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Loan>
            {
                new() { Id = 9, MemberId = 1, ReturnDate = DateTime.Today }
            });

        var result = await _sut.SoftDeleteAsync(1);

        result.Succeeded.Should().BeTrue();
        member.IsActive.Should().BeFalse();
    }
}
