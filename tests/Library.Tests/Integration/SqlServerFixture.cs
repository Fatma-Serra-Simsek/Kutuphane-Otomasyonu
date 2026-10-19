using Library.Business;
using Library.Business.Services;
using Library.Data;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Tests.Integration;

public sealed class SqlServerFixture : IAsyncLifetime
{
    public const string DefaultPassword = "ChangeMe_StrongPass1!";

    private ServiceProvider? _provider;

    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        var password = Environment.GetEnvironmentVariable("SA_PASSWORD") ?? DefaultPassword;
        ConnectionString = Environment.GetEnvironmentVariable("LIBRARY_TEST_DB")
            ?? $"Server=localhost,1433;Database=LibraryDB_Test;User Id=sa;Password={password};TrustServerCertificate=True;Encrypt=True";

        try
        {
            await using var probe = CreateRawContext();
            await probe.Database.CanConnectAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "LibraryDB_Test'e bağlanılamadı. `docker compose up -d` çalıştırın; SA_PASSWORD / LIBRARY_TEST_DB ortam değişkenlerini kontrol edin.",
                ex);
        }

        await using (var db = CreateRawContext())
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.MigrateAsync();
        }

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddLibraryData(ConnectionString);
        services.AddLibraryBusiness();
        _provider = services.BuildServiceProvider();

        await SeedBaselineAsync();
    }

    public async Task DisposeAsync()
    {
        if (_provider is not null)
            await _provider.DisposeAsync();

        try
        {
            await using var db = CreateRawContext();
            await db.Database.EnsureDeletedAsync();
        }
        catch
        {
            // Teardown best-effort.
        }
    }

    public IServiceScope CreateScope() =>
        (_provider ?? throw new InvalidOperationException("Fixture not initialized.")).CreateScope();

    public ILoanService GetLoanService(IServiceScope scope) =>
        scope.ServiceProvider.GetRequiredService<ILoanService>();

    public LibraryDbContext GetDb(IServiceScope scope) =>
        scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

    public async Task<SeedIds> SeedBorrowScenarioAsync(
        bool memberActive = true,
        CopyStatus copyStatus = CopyStatus.Available)
    {
        using var scope = CreateScope();
        var db = GetDb(scope);

        var book = await db.Books.AsNoTracking().FirstAsync();
        var staff = await db.Staff.AsNoTracking().FirstAsync();
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var member = new Member
        {
            MemberNumber = $"T-{suffix}",
            FullName = $"Test Üye {suffix}",
            Email = $"test-{suffix}@example.com",
            Phone = "5550000000",
            RegisteredAt = DateTime.Today,
            IsActive = memberActive
        };
        var copy = new BookCopy
        {
            BookId = book.Id,
            Barcode = $"BC-{suffix}",
            Status = copyStatus,
            IsDeleted = false
        };

        db.Members.Add(member);
        db.BookCopies.Add(copy);
        await db.SaveChangesAsync();

        return new SeedIds(member.Id, copy.Id, staff.Id, copy.Barcode);
    }

    private async Task SeedBaselineAsync()
    {
        using var scope = CreateScope();
        var db = GetDb(scope);

        if (!await db.Settings.AnyAsync())
        {
            db.Settings.Add(new Setting { Key = LoanService.LoanPeriodDaysKey, Value = "14" });
            var category = new Category { Name = "Test Kategori" };
            var publisher = new Publisher { Name = "Test Yayınevi" };
            db.Categories.Add(category);
            db.Publishers.Add(publisher);
            await db.SaveChangesAsync();

            db.Books.Add(new Book
            {
                Title = "Test Kitap",
                ISBN = "9780000000001",
                PublishYear = 2020,
                Description = "Entegrasyon",
                CategoryId = category.Id,
                PublisherId = publisher.Id
            });
            db.Staff.Add(new Staff
            {
                FullName = "Test Personel",
                Email = "staff-test@library.local",
                Role = Role.Librarian
            });
            await db.SaveChangesAsync();
        }
    }

    private LibraryDbContext CreateRawContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;
        return new LibraryDbContext(options);
    }

    public sealed record SeedIds(int MemberId, int CopyId, int StaffId, string Barcode);
}

[CollectionDefinition("SqlServer")]
public class SqlServerCollection : ICollectionFixture<SqlServerFixture>;
