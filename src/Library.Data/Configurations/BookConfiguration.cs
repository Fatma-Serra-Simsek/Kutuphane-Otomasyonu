using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ISBN).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(4000);

        builder.HasIndex(x => x.ISBN);
        builder.HasIndex(x => x.Title);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Books)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Publisher)
            .WithMany(x => x.Books)
            .HasForeignKey(x => x.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
