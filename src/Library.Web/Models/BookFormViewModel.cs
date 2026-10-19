using System.ComponentModel.DataAnnotations;

namespace Library.Web.Models;

public class BookFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    [Display(Name = "Başlık")]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "ISBN zorunludur.")]
    [Display(Name = "ISBN")]
    [StringLength(32)]
    public string ISBN { get; set; } = string.Empty;

    [Display(Name = "Yayın yılı")]
    [Range(1000, 2100)]
    public int PublishYear { get; set; }

    [Display(Name = "Açıklama")]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Kapak görseli URL")]
    [StringLength(500)]
    public string? CoverImageUrl { get; set; }

    [Display(Name = "Kategori")]
    [Range(1, int.MaxValue, ErrorMessage = "Kategori seçin.")]
    public int CategoryId { get; set; }

    [Display(Name = "Yayınevi")]
    [Range(1, int.MaxValue, ErrorMessage = "Yayınevi seçin.")]
    public int PublisherId { get; set; }

    [Display(Name = "Yazarlar")]
    public List<int> AuthorIds { get; set; } = new();
}
