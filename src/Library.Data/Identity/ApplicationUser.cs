using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Library.Data.Identity;

public class ApplicationUser : IdentityUser
{
    public int? StaffId { get; set; }
    public Staff? Staff { get; set; }
}
