using Microsoft.EntityFrameworkCore;

namespace PhotoManagerAPI.DataAccess.Repositories;

public interface IRepository<T, TIdentifier> where T: class
{
    DbSet<T> EntitySet { get; set; }

    Task<bool> CreateAsync(T entity);

    ValueTask<T?> GetAsync(TIdentifier id);
}