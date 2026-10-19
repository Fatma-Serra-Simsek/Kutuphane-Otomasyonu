using Library.Business.Dtos;
using Library.Business.Services;
using Library.Domain.Common;
using Library.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class MembersController : Controller
{
    private readonly IMemberService _members;

    public MembersController(IMemberService members) => _members = members;

    public async Task<IActionResult> Index(string? term, string? status, int page = 1, CancellationToken cancellationToken = default)
    {
        bool? isActive = status switch
        {
            "active" => true,
            "passive" => false,
            _ => null
        };

        var result = await _members.SearchAsync(new SearchQuery
        {
            Term = term,
            IsActive = isActive,
            Page = page,
            PageSize = SearchQuery.DefaultPageSize
        }, cancellationToken);

        return View(new MemberListViewModel
        {
            Term = term,
            Status = status,
            Result = result
        });
    }

    [HttpGet]
    public IActionResult Create() => View(new MemberFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MemberFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _members.CreateAsync(new MemberCreateDto
        {
            MemberNumber = model.MemberNumber,
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone
        }, cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Kayıt başarısız.");
            return View(model);
        }

        TempData["Success"] = "Üye eklendi.";
        return RedirectToAction(nameof(Details), new { id = result.Value!.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var member = await _members.GetByIdAsync(id, cancellationToken);
        if (member is null)
            return NotFound();

        return View(new MemberFormViewModel
        {
            Id = member.Id,
            MemberNumber = member.MemberNumber,
            FullName = member.FullName,
            Email = member.Email,
            Phone = member.Phone,
            IsActive = member.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MemberFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var result = await _members.UpdateAsync(new MemberUpdateDto
        {
            Id = model.Id,
            MemberNumber = model.MemberNumber,
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone,
            IsActive = model.IsActive
        }, cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Güncelleme başarısız.");
            return View(model);
        }

        TempData["Success"] = "Üye güncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var member = await _members.GetByIdAsync(id, cancellationToken);
        if (member is null)
            return NotFound();

        var loans = await _members.GetLoansAsync(id, cancellationToken);
        return View(new MemberDetailsViewModel
        {
            Member = member,
            Loans = loans
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(int id, CancellationToken cancellationToken)
    {
        var result = await _members.SoftDeleteAsync(id, cancellationToken);
        if (!result.Succeeded)
            TempData["Error"] = result.Error;
        else
            TempData["Success"] = "Üye pasife alındı.";

        return RedirectToAction(nameof(Index));
    }
}
