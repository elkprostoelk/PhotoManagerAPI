using Microsoft.EntityFrameworkCore;

namespace PhotoManagerAPI.DataAccess.Repositories;

public interface IRepository<T> where T: class
{
    Task<bool> CreateAsync(T entity);
}