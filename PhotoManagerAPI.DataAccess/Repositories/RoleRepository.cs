using Microsoft.EntityFrameworkCore;
using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.DataAccess.Repositories;

public class RoleRepository(PhotoManagerDbContext dbContext) : Repository<Role, int>(dbContext), IRoleRepository
{
    public async Task<int?> GetAsync(string name) =>
        (await EntitySet.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == name))?.Id;
}