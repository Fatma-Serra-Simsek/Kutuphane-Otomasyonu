using Library.Domain.Common;
using Library.Domain.Entities;

namespace Library.Web.Models;

public class MemberListViewModel
{
    public string? Term { get; init; }
    public string? Status { get; init; }
    public PagedResult<Member> Result { get; init; } = new();
}
