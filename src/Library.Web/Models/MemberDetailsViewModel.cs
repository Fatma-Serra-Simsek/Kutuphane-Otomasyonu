using Library.Domain.Entities;

namespace Library.Web.Models;

public class MemberDetailsViewModel
{
    public Member Member { get; init; } = null!;
    public IReadOnlyList<Loan> Loans { get; init; } = Array.Empty<Loan>();
}
