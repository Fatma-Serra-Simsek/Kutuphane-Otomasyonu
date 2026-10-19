using FluentAssertions;
using Library.Data;
using Library.Data.Repositories;
using Library.Domain.Common;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Library.Tests.Data;

public class RepositoryCrudTests : IDisposable
{
    private readonly LibraryDbContext _db;
    private readonly UnitOfWork _uow;

    public RepositoryCrudTests()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new LibraryDbContext(options);
        _uow = new UnitOfWork(
            _db,
            new BookRepository(_db),
            new BookCopyRepository(_db),
            new MemberRepository(_db),
            new LoanRepository(_db),
            new AuthorRepository(_db),
            new PublisherRepository(_db),
            new CategoryRepository(_db),
            new SettingRepository(_db),
            new StaffRepository(_db));
    }

    [Fact]
    public async Task BookAndMember_Crud_Works()
    {
        var category = new Category { Name = "Roman" };
        var publisher = new Publisher { Name = "Test Yayınevi" };
        _uow.Categories.Add(category);
        _uow.Publishers.Add(publisher);
        await _uow.SaveChangesAsync();

        var book = new Book
        {
            Title = "Test Kitap",
            ISBN = "1234567890",
            PublishYear = 2020,
            Description = "Açıklama",
            CategoryId = category.Id,
            PublisherId = publisher.Id
        };
        _uow.Books.Add(book);

        var member = new Member
        {
            MemberNumber = "M-1",
            FullName = "Ali Veli",
            Email = "ali@example.com",
            Phone = "0555",
            RegisteredAt = DateTime.Today,
            IsActive = true
        };
        _uow.Members.Add(member);
        await _uow.SaveChangesAsync();

        var loadedBook = await _uow.Books.GetByIdAsync(book.Id);
        loadedBook.Should().NotBeNull();
        loadedBook!.Title.Should().Be("Test Kitap");

        var loadedMember = await _uow.Members.GetByIdAsync(member.Id);
        loadedMember.Should().NotBeNull();
        loadedMember!.FullName.Should().Be("Ali Veli");

        book.Title = "Güncel Başlık";
        _uow.Books.Update(book);
        member.FullName = "Ali Veli Güncel";
        _uow.Members.Update(member);
        await _uow.SaveChangesAsync();

        (await _uow.Books.GetByIdAsync(book.Id))!.Title.Should().Be("Güncel Başlık");
        (await _uow.Members.GetByIdAsync(member.Id))!.FullName.Should().Be("Ali Veli Güncel");
    }

    [Fact]
    public async Task LoanRepository_GetActiveByCopy_ReturnsOpenLoan()
    {
        var category = new Category { Name = "Roman" };
        var publisher = new Publisher { Name = "Yayın" };
        var staff = new Staff { FullName = "Personel", Email = "p@lib.local", Role = Role.Librarian };
        var member = new Member
        {
            MemberNumber = "M-2",
            FullName = "Üye",
            Email = "u@ex.com",
            Phone = "1",
            RegisteredAt = DateTime.Today,
            IsActive = true
        };
        _db.AddRange(category, publisher, staff, member);
        await _db.SaveChangesAsync();

        var book = new Book
        {
            Title = "Kitap",
            ISBN = "111",
            PublishYear = 2021,
            CategoryId = category.Id,
            PublisherId = publisher.Id
        };
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var copy = new BookCopy
        {
            BookId = book.Id,
            Barcode = "BC-X",
            Status = CopyStatus.Loaned
        };
        _db.BookCopies.Add(copy);
        await _db.SaveChangesAsync();

        _uow.Loans.Add(new Loan
        {
            BookCopyId = copy.Id,
            MemberId = member.Id,
            StaffId = staff.Id,
            LoanDate = DateTime.Today.AddDays(-3),
            DueDate = DateTime.Today.AddDays(11),
            ReturnDate = null
        });
        await _uow.SaveChangesAsync();

        var active = await _uow.Loans.GetActiveByCopyAsync(copy.Id);
        active.Should().NotBeNull();
        active!.ReturnDate.Should().BeNull();
    }

    [Fact]
    public async Task BookRepository_Search_FiltersByTitle()
    {
        var category = new Category { Name = "Roman" };
        var publisher = new Publisher { Name = "Yayın" };
        _db.AddRange(category, publisher);
        await _db.SaveChangesAsync();

        _uow.Books.Add(new Book
        {
            Title = "Kırmızı Kitap",
            ISBN = "1",
            PublishYear = 2000,
            CategoryId = category.Id,
            PublisherId = publisher.Id
        });
        _uow.Books.Add(new Book
        {
            Title = "Mavi Defter",
            ISBN = "2",
            PublishYear = 2001,
            CategoryId = category.Id,
            PublisherId = publisher.Id
        });
        await _uow.SaveChangesAsync();

        var result = await _uow.Books.SearchAsync(new SearchQuery { Term = "Kırmızı" });
        result.TotalCount.Should().Be(1);
        result.Items.Single().Title.Should().Be("Kırmızı Kitap");
    }

    public void Dispose() => _db.Dispose();
}
