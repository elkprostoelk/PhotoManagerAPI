using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.DataAccess;

public class PhotoManagerDbContext(DbContextOptions<PhotoManagerDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}