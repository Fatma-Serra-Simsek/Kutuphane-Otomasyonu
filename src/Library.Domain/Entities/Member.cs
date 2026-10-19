namespace Library.Domain.Entities;

public class Member
{
    public int Id { get; set; }
    public string MemberNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public bool CanBorrow() => IsActive;
}
