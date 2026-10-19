using System.Security.Claims;

namespace Library.Web.Security;

public static class StaffClaims
{
    public static int? GetStaffId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(LibraryClaimTypes.StaffId);
        return int.TryParse(value, out var id) ? id : null;
    }
}
