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
            await SeedCatalogAsync(db);
            logger.LogInformation("Catalog seed completed.");
        }
        else
        {
            logger.LogInformation("Catalog seed skipped; database already has data.");
        }

        await EnsureIdentityAsync(sp, db, logger);

        logger.LogInformation(
            "Identity ready (development). Seed accounts: {AdminEmail}, {LibrarianEmail}. Passwords are documented in README only.",
            AdminEmail,
            LibrarianEmail);
    }

    private static async Task SeedCatalogAsync(LibraryDbContext db)
    {
        db.Settings.Add(new Setting { Key = "LoanPeriodDays", Value = "14" });

        var categories = new[]
        {
            new Category { Name = "Roman" },
            new Category { Name = "Bilim" },
            new Category { Name = "Tarih" }
        };
        db.Categories.AddRange(categories);

        var publishers = new[]
        {
            new Publisher { Name = "Yapı Kredi Yayınları" },
            new Publisher { Name = "Tübitak Yayınları" }
        };
        db.Publishers.AddRange(publishers);

        var authors = new[]
        {
            new Author { FullName = "Orhan Pamuk" },
            new Author { FullName = "Aziz Sancar" },
            new Author { FullName = "İlber Ortaylı" }
        };
        db.Authors.AddRange(authors);

        await db.SaveChangesAsync();

        var book1 = new Book
        {
            Title = "Masumiyet Müzesi",
            ISBN = "9789750804557",
            PublishYear = 2008,
            Description = "İstanbul'da geçen bir aşk romanı.",
            CategoryId = categories[0].Id,
            PublisherId = publishers[0].Id
        };
        var book2 = new Book
        {
            Title = "Yaşamın Sırrı DNA",
            ISBN = "9789754031234",
            PublishYear = 2015,
            Description = "DNA ve genetik üzerine giriş.",
            CategoryId = categories[1].Id,
            PublisherId = publishers[1].Id
        };
        var book3 = new Book
        {
            Title = "Osmanlı'yı Yeniden Keşfetmek",
            ISBN = "9789750809999",
            PublishYear = 2010,
            Description = "Osmanlı tarihi denemeleri.",
            CategoryId = categories[2].Id,
            PublisherId = publishers[0].Id
        };

        db.Books.AddRange(book1, book2, book3);
        await db.SaveChangesAsync();

        db.BookAuthors.AddRange(
            new BookAuthor { BookId = book1.Id, AuthorId = authors[0].Id },
            new BookAuthor { BookId = book2.Id, AuthorId = authors[1].Id },
            new BookAuthor { BookId = book3.Id, AuthorId = authors[2].Id });

        db.BookCopies.AddRange(
            new BookCopy { BookId = book1.Id, Barcode = "BC-0001", Status = CopyStatus.Available },
            new BookCopy { BookId = book1.Id, Barcode = "BC-0002", Status = CopyStatus.Available },
            new BookCopy { BookId = book2.Id, Barcode = "BC-0003", Status = CopyStatus.Available },
            new BookCopy { BookId = book3.Id, Barcode = "BC-0004", Status = CopyStatus.Available });

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
