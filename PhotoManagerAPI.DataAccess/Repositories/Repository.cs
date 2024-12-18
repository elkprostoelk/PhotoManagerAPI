using Microsoft.EntityFrameworkCore;

namespace PhotoManagerAPI.DataAccess.Repositories;

public class Repository<T, TIdentifier> : IRepository<T, TIdentifier> where T : class
{
    protected readonly PhotoManagerDbContext DbContext;

    public Repository(PhotoManagerDbContext dbContext)
    {
        DbContext = dbContext;
        EntitySet = dbContext.Set<T>();
    }

    public DbSet<T> EntitySet { get; set; }

    public async Task<bool> CreateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        await EntitySet.AddAsync(entity);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async ValueTask<T?> GetAsync(TIdentifier id) =>
        await EntitySet.FindAsync(id);

    public async Task<bool> RemoveAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        EntitySet.Remove(entity);
        return await DbContext.SaveChangesAsync() > 0;
    }
}