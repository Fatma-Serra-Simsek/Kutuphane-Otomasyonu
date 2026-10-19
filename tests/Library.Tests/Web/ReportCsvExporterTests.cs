using System.Text;
using FluentAssertions;
using Library.Business.Dtos;
using Library.Domain.Common;
using Library.Web.Services;

namespace Library.Tests.Web;

public class ReportCsvExporterTests
{
    [Fact]
    public void ExportMostBorrowed_IncludesBomAndTurkishHeaders()
    {
        var bytes = ReportCsvExporter.ExportMostBorrowed(new[]
        {
            new BookBorrowStat { BookId = 1, Title = "Masumiyet Müzesi", BorrowCount = 3 }
        });

        bytes.Should().StartWith(Encoding.UTF8.GetPreamble());
        var text = Encoding.UTF8.GetString(bytes);
        text.Should().Contain("Kitap");
        text.Should().Contain("Ödünç sayısı");
        text.Should().Contain("Masumiyet Müzesi");
        text.Should().Contain("3");
    }

    [Fact]
    public void ExportLoans_EmptyList_StillWritesHeader()
    {
        var bytes = ReportCsvExporter.ExportLoans(Array.Empty<LoanListItemDto>());
        var text = Encoding.UTF8.GetString(bytes);
        text.Should().Contain("Ödünç tarihi");
        text.Should().Contain("Barkod");
        text.Should().Contain("Durum");
    }

    [Fact]
    public void ExportLoans_FormatsDatesAndStatus()
    {
        var bytes = ReportCsvExporter.ExportLoans(new[]
        {
            new LoanListItemDto
            {
                LoanId = 1,
                BookTitle = "Test",
                Barcode = "BC-1",
                MemberName = "Ayşe",
                MemberNumber = "M-1",
                LoanDate = new DateTime(2026, 1, 5),
                DueDate = new DateTime(2026, 1, 19),
                ReturnDate = null,
                IsOverdue = true
            }
        });

        var text = Encoding.UTF8.GetString(bytes);
        text.Should().Contain("05.01.2026");
        text.Should().Contain("19.01.2026");
        text.Should().Contain("Gecikmiş");
        text.Should().Contain("BC-1");
    }
}
