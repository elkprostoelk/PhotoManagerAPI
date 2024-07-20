using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.Name)
            .IsUnique();

        builder.Property(u => u.FullName)
            .HasMaxLength(100);

        builder.Property(u => u.Salt)
            .IsRequired();
        
        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.CreationDate)
            .IsRequired();

        builder.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .IsRequired();
    }
}