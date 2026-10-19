namespace Library.Business.Dtos;

public class BookCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int PublishYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int PublisherId { get; set; }
    public IList<int> AuthorIds { get; set; } = new List<int>();
}
