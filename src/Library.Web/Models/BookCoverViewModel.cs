namespace Library.Web.Models;

public class BookCoverViewModel
{
    public string? Url { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CssClass { get; set; } = "book-cover";
}
