using Library.Business.Services;
using Library.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    private readonly ISettingsService _settings;

    public SettingsController(ISettingsService settings) => _settings = settings;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var days = await _settings.GetLoanPeriodDaysAsync(cancellationToken);
        return View(new SettingsViewModel { LoanPeriodDays = days });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SettingsViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _settings.SetLoanPeriodDaysAsync(model.LoanPeriodDays, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Kayıt başarısız.");
            return View(model);
        }

        TempData["Success"] = "Ödünç süresi güncellendi.";
        return RedirectToAction(nameof(Index));
    }
}
