using System.ComponentModel.DataAnnotations;

namespace Library.Web.Models;

public class ReturnViewModel
{
    [Required(ErrorMessage = "Barkod zorunludur.")]
    [Display(Name = "Barkod")]
    public string Barcode { get; set; } = string.Empty;
}
