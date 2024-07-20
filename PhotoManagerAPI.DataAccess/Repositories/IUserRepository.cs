using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.DataAccess.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<bool> ExistsAsync(string userName);
}