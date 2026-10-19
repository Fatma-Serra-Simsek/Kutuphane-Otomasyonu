using System.ComponentModel.DataAnnotations;

namespace Library.Web.Models;

public class MemberFormViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Üye no")]
    [StringLength(32)]
    public string MemberNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Ad soyad")]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "E-posta")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Telefon")]
    [StringLength(32)]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;
}
