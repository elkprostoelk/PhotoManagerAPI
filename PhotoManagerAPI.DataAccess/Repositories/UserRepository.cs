using Microsoft.EntityFrameworkCore;
using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.DataAccess.Repositories;

public class UserRepository(PhotoManagerDbContext dbContext) : Repository<User>(dbContext), IUserRepository
{
    public async Task<bool> ExistsAsync(string userName) =>
        await EntitySet
            .AsNoTracking()
            .AnyAsync(u => u.Name == userName);
}