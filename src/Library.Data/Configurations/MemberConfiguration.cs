using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MemberNumber).HasMaxLength(32).IsRequired();
        builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(32);
        builder.Property(x => x.RegisteredAt).IsRequired();

        builder.HasIndex(x => x.MemberNumber).IsUnique();
        builder.HasIndex(x => x.FullName);
    }
}
