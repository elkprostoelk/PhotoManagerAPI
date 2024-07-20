using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.DataAccess.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(15);

        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.HasData([
            new Role {Id = 1, Name = "Admin"},
            new Role {Id = 2, Name = "Moderator"},
            new Role {Id = 3, Name = "User"}
        ]);
    }
}