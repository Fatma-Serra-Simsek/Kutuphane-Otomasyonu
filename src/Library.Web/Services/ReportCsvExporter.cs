using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Library.Business.Dtos;
using Library.Domain.Common;

namespace Library.Web.Services;

public static class ReportCsvExporter
{
    private static readonly CultureInfo Tr = CultureInfo.GetCultureInfo("tr-TR");

    public static byte[] ExportMostBorrowed(IEnumerable<BookBorrowStat> rows) =>
        Write(rows.Select(r => new MostBorrowedCsvRow
        {
            Title = r.Title,
            BorrowCount = r.BorrowCount
        }));

    public static byte[] ExportTopMembers(IEnumerable<MemberBorrowStat> rows) =>
        Write(rows.Select(r => new TopMemberCsvRow
        {
            FullName = r.FullName,
            MemberNumber = r.MemberNumber,
            BorrowCount = r.BorrowCount
        }));

    public static byte[] ExportLoans(IEnumerable<LoanListItemDto> rows) =>
        Write(rows.Select(r => new LoanCsvRow
        {
            LoanDate = r.LoanDate.ToString("dd.MM.yyyy", Tr),
            DueDate = r.DueDate.ToString("dd.MM.yyyy", Tr),
            ReturnDate = r.ReturnDate?.ToString("dd.MM.yyyy", Tr) ?? string.Empty,
            MemberName = r.MemberName,
            MemberNumber = r.MemberNumber,
            BookTitle = r.BookTitle,
            Barcode = r.Barcode,
            Status = r.ReturnDate is not null
                ? "İade"
                : r.IsOverdue
                    ? "Gecikmiş"
                    : "Ödünçte"
        }));

    private static byte[] Write<T>(IEnumerable<T> rows)
    {
        using var memory = new MemoryStream();
        var preamble = Encoding.UTF8.GetPreamble();
        memory.Write(preamble, 0, preamble.Length);

        using (var writer = new StreamWriter(memory, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        }))
        {
            csv.WriteRecords(rows);
            writer.Flush();
        }

        return memory.ToArray();
    }

    private sealed class MostBorrowedCsvRow
    {
        [Name("Kitap")]
        public string Title { get; set; } = string.Empty;

        [Name("Ödünç sayısı")]
        public int BorrowCount { get; set; }
    }

    private sealed class TopMemberCsvRow
    {
        [Name("Üye")]
        public string FullName { get; set; } = string.Empty;

        [Name("Üye no")]
        public string MemberNumber { get; set; } = string.Empty;

        [Name("Ödünç sayısı")]
        public int BorrowCount { get; set; }
    }

    private sealed class LoanCsvRow
    {
        [Name("Ödünç tarihi")]
        public string LoanDate { get; set; } = string.Empty;

        [Name("Teslim tarihi")]
        public string DueDate { get; set; } = string.Empty;

        [Name("İade tarihi")]
        public string ReturnDate { get; set; } = string.Empty;

        [Name("Üye")]
        public string MemberName { get; set; } = string.Empty;

        [Name("Üye no")]
        public string MemberNumber { get; set; } = string.Empty;

        [Name("Kitap")]
        public string BookTitle { get; set; } = string.Empty;

        [Name("Barkod")]
        public string Barcode { get; set; } = string.Empty;

        [Name("Durum")]
        public string Status { get; set; } = string.Empty;
    }
}
