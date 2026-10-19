using FluentAssertions;
using Library.Domain.Entities;
using Library.Domain.Enums;

namespace Library.Tests.Domain;

public class BookTests
{
    [Fact]
    public void GetAvailableCopyCount_CountsOnlyNonDeletedAvailableCopies()
    {
        var book = new Book
        {
            Copies =
            {
                new BookCopy { Status = CopyStatus.Available, IsDeleted = false },
                new BookCopy { Status = CopyStatus.Available, IsDeleted = true },
                new BookCopy { Status = CopyStatus.Loaned, IsDeleted = false },
                new BookCopy { Status = CopyStatus.Damaged, IsDeleted = false },
                new BookCopy { Status = CopyStatus.Available, IsDeleted = false }
            }
        };

        book.GetAvailableCopyCount().Should().Be(2);
    }
}
