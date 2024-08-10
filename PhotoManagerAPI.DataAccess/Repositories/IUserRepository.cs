using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.DataAccess.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<bool> ExistsAsync(string userName);
    Task<bool> ExistsAsync(Guid id);
    Task<User?> GetAsync(string userNameOrEmail);
}