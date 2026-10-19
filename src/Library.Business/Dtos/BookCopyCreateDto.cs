namespace Library.Business.Dtos;

public class BookCopyCreateDto
{
    public int BookId { get; set; }
    public string Barcode { get; set; } = string.Empty;
}
