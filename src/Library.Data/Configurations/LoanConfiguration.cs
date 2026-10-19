using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Data.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LoanDate).IsRequired();
        builder.Property(x => x.DueDate).IsRequired();

        // Matches BookCopy soft-delete query filter (EF warning 10622).
        builder.HasQueryFilter(x => !x.BookCopy.IsDeleted);

        builder.HasIndex(x => x.BookCopyId)
            .IsUnique()
            .HasFilter("[ReturnDate] IS NULL")
            .HasDatabaseName("UX_Loans_ActiveCopy");

        builder.HasOne(x => x.Member)
            .WithMany(x => x.Loans)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BookCopy)
            .WithMany(x => x.Loans)
            .HasForeignKey(x => x.BookCopyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Staff)
            .WithMany(x => x.Loans)
            .HasForeignKey(x => x.StaffId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
