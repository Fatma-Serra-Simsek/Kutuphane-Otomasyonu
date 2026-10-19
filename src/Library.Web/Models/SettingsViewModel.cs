using System.ComponentModel.DataAnnotations;

namespace Library.Web.Models;

public class SettingsViewModel
{
    [Display(Name = "Ödünç süresi (gün)")]
    [Range(1, 365, ErrorMessage = "1–365 gün arası olmalıdır.")]
    public int LoanPeriodDays { get; set; }
}
