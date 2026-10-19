using System.ComponentModel.DataAnnotations;

namespace Library.Web.Models;

public class BorrowViewModel
{
    [Display(Name = "Üye")]
    [Range(1, int.MaxValue, ErrorMessage = "Üye seçin.")]
    public int MemberId { get; set; }

    [Required(ErrorMessage = "Barkod zorunludur.")]
    [Display(Name = "Barkod")]
    public string Barcode { get; set; } = string.Empty;

    public int LoanPeriodDays { get; set; }
    public DateTime PreviewDueDate { get; set; }
}
