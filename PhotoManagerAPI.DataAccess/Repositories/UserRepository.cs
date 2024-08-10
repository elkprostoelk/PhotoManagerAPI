using Microsoft.EntityFrameworkCore;
using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.DataAccess.Repositories;

public class UserRepository(PhotoManagerDbContext dbContext) : Repository<User, Guid>(dbContext), IUserRepository
{
    public async Task<bool> ExistsAsync(string userName) =>
        await EntitySet
            .AsNoTracking()
            .AnyAsync(u => u.Name == userName);

    public async Task<bool> ExistsAsync(Guid id) =>
        await EntitySet
            .AsNoTracking()
            .AnyAsync(u => u.Id == id);

    public async Task<User?> GetAsync(string userNameOrEmail) =>
        await EntitySet
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == userNameOrEmail
                || u.Name == userNameOrEmail);
}