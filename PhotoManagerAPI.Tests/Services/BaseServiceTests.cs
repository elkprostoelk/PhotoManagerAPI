using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using PhotoManagerAPI.DataAccess;
using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.Tests.Services;

public class BaseServiceTests
{
    protected readonly PhotoManagerDbContext DbContext;

    protected BaseServiceTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<PhotoManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        DbContext = new PhotoManagerDbContext(dbContextOptions);

        DbContext.Roles.AddRange([
            new Role {Id = 1, Name = "Admin"},
            new Role {Id = 2, Name = "Moderator"},
            new Role {Id = 3, Name = "User"}
        ]);
        DbContext.Users.AddRange([
            new User
            {
                Id = Guid.Parse("0190d1a3-4389-98ff-816b-a83e4376af7f"),
                Name = "UserName",
                Email = "username@mail.com",
                FullName = "User Middle Name",
                CreationDate = DateTime.UtcNow,
                PasswordHash = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256/8)),
                Salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128/8)),
                RoleId = 1
            },
            new User
            {
                Id = Guid.Parse("0190d1a3-439c-d4e5-d362-32ca66912a88"),
                Name = "a_never",
                Email = "alexnevr@mail.com",
                FullName = "Alex Carter Never",
                CreationDate = DateTime.UtcNow,
                PasswordHash = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256/8)),
                Salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128/8)),
                RoleId = 2
            },
            new User
            {
                Id = Guid.Parse("0190d1a3-439c-d68a-2636-7c9af6a8600b"),
                Name = "sall_nev3r",
                Email = "sallynvr1234@mail.com",
                FullName = "Sally Middle Never",
                CreationDate = DateTime.UtcNow,
                PasswordHash = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256/8)),
                Salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128/8)),
                RoleId = 3
            },
            new User
            {
                Id = Guid.Parse("0190d1a3-439c-b416-92d3-6fd43e46ca0f"),
                Name = "mikenvr1345",
                Email = "mike.never@mail.com",
                FullName = "Michael Alex Never",
                CreationDate = DateTime.UtcNow,
                PasswordHash = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256/8)),
                Salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128/8)),
                RoleId = 3
            },
            new User
            {
                Id = Guid.Parse("0190d1a3-439c-9fd9-941b-425cb494d8a4"),
                Name = "mrghost-annyhilator",
                Email = "alwsann@mail.com",
                FullName = "Ann Middle Always",
                CreationDate = DateTime.UtcNow,
                PasswordHash = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256/8)),
                Salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128/8)),
                RoleId = 3
            }
        ]);
        DbContext.SaveChanges();
    }
}