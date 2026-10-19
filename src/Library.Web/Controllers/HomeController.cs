using System.Diagnostics;
using Library.Business.Services;
using Library.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class HomeController : Controller
{
    private readonly IDashboardService _dashboard;

    public HomeController(IDashboardService dashboard)
    {
        _dashboard = dashboard;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var summary = await _dashboard.GetSummaryAsync(cancellationToken);
        var recent = await _dashboard.GetRecentLoansAsync(5, cancellationToken);
        return View(new DashboardViewModel
        {
            Summary = summary,
            RecentLoans = recent
        });
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
