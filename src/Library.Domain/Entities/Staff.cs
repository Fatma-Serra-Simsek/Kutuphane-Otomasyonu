using Library.Domain.Enums;

namespace Library.Domain.Entities;

public class Staff
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Role Role { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
