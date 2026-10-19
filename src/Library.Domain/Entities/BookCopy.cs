using Library.Domain.Enums;

namespace Library.Domain.Entities;

public class BookCopy
{
    public int Id { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public CopyStatus Status { get; set; }
    public int BookId { get; set; }
    public bool IsDeleted { get; set; }

    public Book Book { get; set; } = null!;
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
