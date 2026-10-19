namespace Library.Business.Dtos;

public class BookUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int PublishYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public int CategoryId { get; set; }
    public int PublisherId { get; set; }
    public IList<int> AuthorIds { get; set; } = new List<int>();
}
