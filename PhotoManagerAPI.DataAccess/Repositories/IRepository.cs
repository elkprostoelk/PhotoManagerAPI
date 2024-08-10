using Microsoft.EntityFrameworkCore;

namespace PhotoManagerAPI.DataAccess.Repositories;

public interface IRepository<T, TIdentifier> where T: class
{
    Task<bool> CreateAsync(T entity);

    ValueTask<T?> GetAsync(TIdentifier id);
}