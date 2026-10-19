using System.ComponentModel.DataAnnotations;
using Library.Business.Dtos;

namespace Library.Web.Models;

public class ReportDateRangeViewModel
{
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Başlangıç")]
    public DateTime From { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Bitiş")]
    public DateTime To { get; set; }

    public IReadOnlyList<LoanListItemDto>? Results { get; set; }
}
