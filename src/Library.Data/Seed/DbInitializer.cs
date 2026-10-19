using Library.Data.Identity;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Library.Data.Seed;

public static class DbInitializer
{
    public const string AdminEmail = "admin@library.local";
    public const string AdminPassword = "Admin123!";
    public const string LibrarianEmail = "librarian@library.local";
    public const string LibrarianPassword = "Librarian123!";
    public const string AdminRoleName = "Admin";
    public const string LibrarianRoleName = "Librarian";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");
        var db = sp.GetRequiredService<LibraryDbContext>();

        await db.Database.MigrateAsync();

        if (!await db.Settings.AnyAsync())
        {
            await SeedCatalogAsync(db, logger);
            logger.LogInformation("Catalog seed completed.");
        }
        else
        {
            logger.LogInformation("Catalog seed skipped; database already has data.");
        }

        await EnsureDemoCatalogAsync(db, logger);
        await EnsureIdentityAsync(sp, db, logger);

        logger.LogInformation(
            "Identity ready (development). Seed accounts: {AdminEmail}, {LibrarianEmail}. Passwords are documented in README only.",
            AdminEmail,
            LibrarianEmail);
    }

    private static async Task SeedCatalogAsync(LibraryDbContext db, ILogger logger)
    {
        db.Settings.Add(new Setting { Key = "LoanPeriodDays", Value = "14" });

        db.Members.Add(new Member
        {
            MemberNumber = "M-1001",
            FullName = "Ayşe Yılmaz",
            Email = "ayse.yilmaz@example.com",
            Phone = "05551234567",
            RegisteredAt = DateTime.Today,
            IsActive = true
        });

        db.Staff.Add(new Staff
        {
            FullName = "Sistem Yöneticisi",
            Email = AdminEmail,
            Role = Role.Admin
        });

        db.Staff.Add(new Staff
        {
            FullName = "Kütüphane Personeli",
            Email = LibrarianEmail,
            Role = Role.Librarian
        });

        await db.SaveChangesAsync();
        await EnsureDemoCatalogAsync(db, logger);
    }

    /// <summary>
    /// Idempotent: eksik kategori/yayınevi/yazar/kitapları ISBN'e göre tamamlar.
    /// Mevcut Development DB'lerinde de 50+ kapaklı kitabı garanti eder.
    /// </summary>
    public static async Task EnsureDemoCatalogAsync(LibraryDbContext db, ILogger logger)
    {
        var categories = await EnsureNamedEntitiesAsync(
            db,
            db.Categories,
            SeedCatalogData.Categories
                .Concat(SeedCatalogData.Books.Select(b => b.Category))
                .Distinct(StringComparer.OrdinalIgnoreCase),
            name => new Category { Name = name },
            c => c.Name);

        var publishers = await EnsureNamedEntitiesAsync(
            db,
            db.Publishers,
            SeedCatalogData.Publishers
                .Concat(SeedCatalogData.Books.Select(b => b.Publisher))
                .Distinct(StringComparer.OrdinalIgnoreCase),
            name => new Publisher { Name = name },
            p => p.Name);

        var authors = await EnsureNamedEntitiesAsync(
            db,
            db.Authors,
            SeedCatalogData.Books.Select(b => b.Author).Distinct(StringComparer.OrdinalIgnoreCase),
            name => new Author { FullName = name },
            a => a.FullName);

        var existingByIsbn = await db.Books
            .IgnoreQueryFilters()
            .ToDictionaryAsync(b => b.ISBN, StringComparer.OrdinalIgnoreCase);

        var nextBarcode = await NextBarcodeNumberAsync(db);
        var added = 0;
        var coversUpdated = 0;

        foreach (var def in SeedCatalogData.Books)
        {
            if (existingByIsbn.TryGetValue(def.Isbn, out var existing))
            {
                if (string.IsNullOrWhiteSpace(existing.CoverImageUrl))
                {
                    existing.CoverImageUrl = SeedCatalogData.CoverUrl(def.Isbn);
                    coversUpdated++;
                }

                continue;
            }

            if (!categories.TryGetValue(def.Category, out var category) ||
                !publishers.TryGetValue(def.Publisher, out var publisher) ||
                !authors.TryGetValue(def.Author, out var author))
            {
                continue;
            }

            var book = new Book
            {
                Title = def.Title,
                ISBN = def.Isbn,
                PublishYear = def.PublishYear,
                Description = def.Description,
                CoverImageUrl = SeedCatalogData.CoverUrl(def.Isbn),
                CategoryId = category.Id,
                PublisherId = publisher.Id
            };
            book.BookAuthors.Add(new BookAuthor { Author = author });
            book.Copies.Add(new BookCopy
            {
                Barcode = $"BC-{nextBarcode:D4}",
                Status = CopyStatus.Available
            });
            nextBarcode++;

            db.Books.Add(book);
            existingByIsbn[def.Isbn] = book;
            added++;
        }

        var missingCovers = await db.Books
            .Where(b => b.CoverImageUrl == null || b.CoverImageUrl == "")
            .ToListAsync();
        foreach (var book in missingCovers)
        {
            if (string.IsNullOrWhiteSpace(book.ISBN))
                continue;
            book.CoverImageUrl = SeedCatalogData.CoverUrl(book.ISBN);
            coversUpdated++;
        }

        if (added > 0 || coversUpdated > 0)
            await db.SaveChangesAsync();

        var total = await db.Books.CountAsync();
        logger.LogInformation(
            "Demo catalog ensured. Added {Added} books, updated {Covers} covers. Total books: {Total}.",
            added,
            coversUpdated,
            total);
    }

    private static async Task<Dictionary<string, T>> EnsureNamedEntitiesAsync<T>(
        LibraryDbContext db,
        DbSet<T> set,
        IEnumerable<string> names,
        Func<string, T> factory,
        Func<T, string> nameSelector)
        where T : class
    {
        var existing = await set.ToListAsync();
        var map = existing.ToDictionary(nameSelector, StringComparer.OrdinalIgnoreCase);
        var created = false;

        foreach (var name in names)
        {
            if (map.ContainsKey(name))
                continue;

            var entity = factory(name);
            set.Add(entity);
            map[name] = entity;
            created = true;
        }

        if (created)
            await db.SaveChangesAsync();

        return map;
    }

    private static async Task<int> NextBarcodeNumberAsync(LibraryDbContext db)
    {
        var barcodes = await db.BookCopies
            .IgnoreQueryFilters()
            .Select(c => c.Barcode)
            .ToListAsync();

        var max = 0;
        foreach (var barcode in barcodes)
        {
            if (barcode.StartsWith("BC-", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(barcode.AsSpan(3), out var n) &&
                n > max)
            {
                max = n;
            }
        }

        return max + 1;
    }

    private static async Task EnsureIdentityAsync(
        IServiceProvider sp,
        LibraryDbContext db,
        ILogger logger)
    {
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var roleName in new[] { AdminRoleName, LibrarianRoleName })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        await EnsureStaffAndUserAsync(
            db,
            userManager,
            logger,
            email: AdminEmail,
            password: AdminPassword,
            fullName: "Sistem Yöneticisi",
            staffRole: Role.Admin,
            identityRole: AdminRoleName);

        await EnsureStaffAndUserAsync(
            db,
            userManager,
            logger,
            email: LibrarianEmail,
            password: LibrarianPassword,
            fullName: "Kütüphane Personeli",
            staffRole: Role.Librarian,
            identityRole: LibrarianRoleName);
    }

    private static async Task EnsureStaffAndUserAsync(
        LibraryDbContext db,
        UserManager<ApplicationUser> userManager,
        ILogger logger,
        string email,
        string password,
        string fullName,
        Role staffRole,
        string identityRole)
    {
        var staff = await db.Staff.FirstOrDefaultAsync(s => s.Email == email);
        if (staff is null)
        {
            staff = new Staff
            {
                FullName = fullName,
                Email = email,
                Role = staffRole
            };
            db.Staff.Add(staff);
            await db.SaveChangesAsync();
        }

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            if (existing.StaffId != staff.Id)
            {
                existing.StaffId = staff.Id;
                await userManager.UpdateAsync(existing);
            }

            if (!await userManager.IsInRoleAsync(existing, identityRole))
                await userManager.AddToRoleAsync(existing, identityRole);

            return;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            StaffId = staff.Id
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            logger.LogError("User seed failed for {Email}: {Errors}", email, errors);
            throw new InvalidOperationException($"User seed failed for {email}: {errors}");
        }

        await userManager.AddToRoleAsync(user, identityRole);
    }
}
