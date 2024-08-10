using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.DataAccess.Configurations;

public class PictureConfiguration : IEntityTypeConfiguration<Picture>
{
    public void Configure(EntityTypeBuilder<Picture> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(p => p.Title);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.PhysicalPath)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(p => p.Width)
            .IsRequired();

        builder.Property(p => p.Height)
            .IsRequired();

        builder.Property(p => p.Iso)
            .HasMaxLength(15);

        builder.Property(p => p.CameraModel)
            .HasMaxLength(20);

        builder.Property(p => p.FocusDistance)
            .HasMaxLength(10);

        builder.Property(p => p.Created)
            .IsRequired();

        builder.HasOne(p => p.User)
            .WithMany(u => u.Pictures)
            .HasForeignKey(p => p.UserId)
            .IsRequired();
    }
}