using Library.Business.Results;
using Library.Business.Services;
using Library.Domain.Common;
using Library.Web.Models;
using Library.Web.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library.Web.Controllers;

public class LoansController : Controller
{
    private readonly ILoanService _loans;
    private readonly IMemberService _members;
    private readonly ISettingsService _settings;

    public LoansController(ILoanService loans, IMemberService members, ISettingsService settings)
    {
        _loans = loans;
        _members = members;
        _settings = settings;
    }

    [HttpGet]
    public async Task<IActionResult> Borrow(CancellationToken cancellationToken)
    {
        var period = await _settings.GetLoanPeriodDaysAsync(cancellationToken);
        await PopulateActiveMembers(cancellationToken);
        return View(new BorrowViewModel
        {
            LoanPeriodDays = period,
            PreviewDueDate = DateTime.Today.AddDays(period)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(BorrowViewModel model, CancellationToken cancellationToken)
    {
        var period = await _settings.GetLoanPeriodDaysAsync(cancellationToken);
        model.LoanPeriodDays = period;
        model.PreviewDueDate = DateTime.Today.AddDays(period);

        if (!ModelState.IsValid)
        {
            await PopulateActiveMembers(cancellationToken);
            return View(model);
        }

        var staffId = User.GetStaffId();
        if (staffId is null)
        {
            TempData["Error"] = "Personel kaydı bulunamadı. Yeniden giriş yapın.";
            await PopulateActiveMembers(cancellationToken);
            return View(model);
        }

        var result = await _loans.BorrowByBarcodeAsync(
            model.MemberId,
            model.Barcode,
            staffId.Value,
            cancellationToken);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message ?? MapLoanCode(result.Code);
            await PopulateActiveMembers(cancellationToken);
            model.Barcode = string.Empty;
            return View(model);
        }

        TempData["Success"] =
            $"Ödünç verildi. Teslim: {result.Loan!.DueDate:dd.MM.yyyy}";
        return RedirectToAction(nameof(Borrow));
    }

    [HttpGet]
    public IActionResult Return() => View(new ReturnViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(ReturnViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _loans.ReturnByBarcodeAsync(model.Barcode, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message ?? MapLoanCode(result.Code);
            model.Barcode = string.Empty;
            return View(model);
        }

        TempData["Success"] = result.DaysOverdue > 0
            ? $"İade alındı. Gecikme: {result.DaysOverdue} gün."
            : "İade alındı.";
        return RedirectToAction(nameof(Return));
    }

    public async Task<IActionResult> Active(CancellationToken cancellationToken)
    {
        var loans = await _loans.GetActiveLoansAsync(cancellationToken);
        return View(loans);
    }

    public async Task<IActionResult> Overdue(CancellationToken cancellationToken)
    {
        var loans = await _loans.GetOverdueLoansAsync(cancellationToken);
        return View(loans);
    }

    private async Task PopulateActiveMembers(CancellationToken cancellationToken)
    {
        var page = await _members.SearchAsync(new SearchQuery
        {
            IsActive = true,
            Page = 1,
            PageSize = 200
        }, cancellationToken);

        ViewBag.Members = page.Items
            .Select(m => new SelectListItem($"{m.FullName} ({m.MemberNumber})", m.Id.ToString()))
            .ToList();
    }

    private static string MapLoanCode(LoanResultCode code) => code switch
    {
        LoanResultCode.MemberNotFound => "Üye bulunamadı.",
        LoanResultCode.MemberInactive => "Üye aktif değil.",
        LoanResultCode.CopyNotFound => "Kopya bulunamadı.",
        LoanResultCode.CopyNotAvailable => "Kopya müsait değil.",
        LoanResultCode.NoActiveLoan => "Aktif ödünç bulunamadı.",
        _ => "İşlem başarısız."
    };
}
