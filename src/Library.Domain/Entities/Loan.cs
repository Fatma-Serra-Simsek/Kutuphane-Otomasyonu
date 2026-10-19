namespace Library.Domain.Entities;

public class Loan
{
    public int Id { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public int MemberId { get; set; }
    public int BookCopyId { get; set; }
    public int StaffId { get; set; }

    public Member Member { get; set; } = null!;
    public BookCopy BookCopy { get; set; } = null!;
    public Staff Staff { get; set; } = null!;

    public bool IsOverdue() =>
        ReturnDate == null && DueDate.Date < DateTime.Today;
}
