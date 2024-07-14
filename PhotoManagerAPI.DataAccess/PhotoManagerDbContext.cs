using Microsoft.EntityFrameworkCore;

namespace PhotoManagerAPI.DataAccess;

public class PhotoManagerDbContext(DbContextOptions<PhotoManagerDbContext> options) : DbContext(options)
{
    
}