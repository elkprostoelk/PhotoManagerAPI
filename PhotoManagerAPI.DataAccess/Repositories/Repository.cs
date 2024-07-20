using Microsoft.EntityFrameworkCore;

namespace PhotoManagerAPI.DataAccess.Repositories;

public class Repository<T> : IRepository<T> where T: class
{
    protected readonly PhotoManagerDbContext _dbContext;

    public Repository(PhotoManagerDbContext dbContext)
    {
        _dbContext = dbContext;
        EntitySet = dbContext.Set<T>();
    }

    public DbSet<T> EntitySet { get; set; }

    public async Task<bool> CreateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        await _dbContext.AddAsync(entity);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}