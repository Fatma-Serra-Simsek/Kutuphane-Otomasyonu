namespace Library.Business.Dtos;

public class LoanListItemDto
{
    public int LoanId { get; init; }
    public string BookTitle { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public string MemberName { get; init; } = string.Empty;
    public string MemberNumber { get; init; } = string.Empty;
    public DateTime LoanDate { get; init; }
    public DateTime DueDate { get; init; }
    public DateTime? ReturnDate { get; init; }
    public bool IsOverdue { get; init; }
}
