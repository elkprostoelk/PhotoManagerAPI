using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.DataAccess.Repositories;

public interface IRoleRepository: IRepository<Role, int>
{
    Task<int?> GetAsync(string name);
}