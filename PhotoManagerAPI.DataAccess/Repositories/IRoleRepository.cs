using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.DataAccess.Repositories;

public interface IRoleRepository: IRepository<Role>
{
    Task<int?> GetAsync(string name);
}