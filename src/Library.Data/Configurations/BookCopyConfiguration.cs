using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Data.Configurations;

public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
{
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        builder.ToTable("BookCopies");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Barcode).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.HasIndex(x => x.Barcode).IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Book)
            .WithMany(x => x.Copies)
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
