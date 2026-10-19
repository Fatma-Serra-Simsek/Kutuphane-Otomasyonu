using Library.Business.Services;
using Library.Web.Models;
using Library.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class ReportsController : Controller
{
    private const string CsvContentType = "text/csv; charset=utf-8";
    private readonly IReportService _reports;

    public ReportsController(IReportService reports) => _reports = reports;

    public IActionResult Index() => View();

    public async Task<IActionResult> MostBorrowed(CancellationToken cancellationToken)
    {
        var data = await _reports.MostBorrowedBooksAsync(10, cancellationToken);
        ViewData["Title"] = "En çok ödünç alınan kitaplar";
        return View(data);
    }

    public async Task<IActionResult> ExportMostBorrowed(CancellationToken cancellationToken)
    {
        var data = await _reports.MostBorrowedBooksAsync(10, cancellationToken);
        var bytes = ReportCsvExporter.ExportMostBorrowed(data);
        return File(bytes, CsvContentType, "en-cok-odunc-kitaplar.csv");
    }

    public async Task<IActionResult> TopMembers(CancellationToken cancellationToken)
    {
        var data = await _reports.TopBorrowingMembersAsync(10, cancellationToken);
        ViewData["Title"] = "En çok kitap alan üyeler";
        return View(data);
    }

    public async Task<IActionResult> ExportTopMembers(CancellationToken cancellationToken)
    {
        var data = await _reports.TopBorrowingMembersAsync(10, cancellationToken);
        var bytes = ReportCsvExporter.ExportTopMembers(data);
        return File(bytes, CsvContentType, "en-cok-kitap-alan-uyeler.csv");
    }

    public async Task<IActionResult> ActiveLoans(CancellationToken cancellationToken)
    {
        var data = await _reports.ActiveLoansAsync(cancellationToken);
        ViewData["Title"] = "Ödünçteki kitaplar (rapor)";
        ViewData["ExportAction"] = "ExportActiveLoans";
        return View("LoanList", data);
    }

    public async Task<IActionResult> ExportActiveLoans(CancellationToken cancellationToken)
    {
        var data = await _reports.ActiveLoansAsync(cancellationToken);
        var bytes = ReportCsvExporter.ExportLoans(data);
        return File(bytes, CsvContentType, "oduncteki-kitaplar.csv");
    }

    public async Task<IActionResult> OverdueLoans(CancellationToken cancellationToken)
    {
        var data = await _reports.OverdueLoansAsync(cancellationToken);
        ViewData["Title"] = "Gecikmiş kitaplar (rapor)";
        ViewData["ExportAction"] = "ExportOverdueLoans";
        return View("LoanList", data);
    }

    public async Task<IActionResult> ExportOverdueLoans(CancellationToken cancellationToken)
    {
        var data = await _reports.OverdueLoansAsync(cancellationToken);
        var bytes = ReportCsvExporter.ExportLoans(data);
        return File(bytes, CsvContentType, "gecikmis-kitaplar.csv");
    }

    [HttpGet]
    public IActionResult ByDateRange()
    {
        return View(new ReportDateRangeViewModel
        {
            From = DateTime.Today.AddMonths(-1),
            To = DateTime.Today
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ByDateRange(ReportDateRangeViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        model.Results = await _reports.LoansByDateRangeAsync(model.From, model.To, cancellationToken);
        ViewData["Title"] = "Tarih aralığı ödünçleri";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ExportByDateRange(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var data = await _reports.LoansByDateRangeAsync(from, to, cancellationToken);
        var bytes = ReportCsvExporter.ExportLoans(data);
        var fileName = $"odunc-{from:yyyyMMdd}-{to:yyyyMMdd}.csv";
        return File(bytes, CsvContentType, fileName);
    }
}
