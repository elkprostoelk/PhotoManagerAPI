using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PhotoManagerAPI.DataAccess;
using PhotoManagerAPI.DataAccess.Entities;

namespace PhotoManagerAPI.Tests.Services;

[ExcludeFromCodeCoverage]
public class BaseServiceTests
{
    private const int HashByteSize = 256 / 8;
    private const int IterationCount = 100_000;

    protected readonly PhotoManagerDbContext DbContext;
    protected readonly IConfiguration Configuration;

    protected BaseServiceTests()
    {
        Configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([
                new KeyValuePair<string, string?>("Jwt:Key", Guid.NewGuid().ToString()),
                new KeyValuePair<string, string?>("Jwt:Issuer", "PhotoManagerApp"),
                new KeyValuePair<string, string?>("Jwt:Audience", "PhotoManagerApp"),
                new KeyValuePair<string, string?>("Jwt:DurationInMinutes", "60"),
                ])
            .Build();
        var dbContextOptions = new DbContextOptionsBuilder<PhotoManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        DbContext = new PhotoManagerDbContext(dbContextOptions);

        DbContext.Roles.AddRange([
            new Role {Id = 1, Name = "Admin"},
            new Role {Id = 2, Name = "Moderator"},
            new Role {Id = 3, Name = "User"}
        ]);

        SeedUsers();

        SeedPictures();
        
        DbContext.SaveChanges();
    }

    private void SeedUsers()
    {
        var salts = Enumerable.Range(0, 5)
            .Select(i => Convert.ToBase64String(RandomNumberGenerator.GetBytes(128 / 8)))
            .ToList();
        DbContext.Users.AddRange([
            new User
            {
                Id = Guid.Parse("0190d1a3-4389-98ff-816b-a83e4376af7f"),
                Name = "UserName",
                Email = "username@mail.com",
                FullName = "User Middle Name",
                CreationDate = DateTime.UtcNow,
                PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: "MyStrongPa$$word1",
                    Convert.FromBase64String(salts[0]),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: IterationCount,
                    numBytesRequested: HashByteSize)),
                Salt = salts[0],
                RoleId = 1
            },
            new User
            {
                Id = Guid.Parse("0190d1a3-439c-d4e5-d362-32ca66912a88"),
                Name = "a_never",
                Email = "alexnevr@mail.com",
                FullName = "Alex Carter Never",
                CreationDate = DateTime.UtcNow,
                PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: "MyStrongPa$$word2",
                    Convert.FromBase64String(salts[1]),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: IterationCount,
                    numBytesRequested: HashByteSize)),
                Salt = salts[1],
                RoleId = 2
            },
            new User
            {
                Id = Guid.Parse("0190d1a3-439c-d68a-2636-7c9af6a8600b"),
                Name = "sall_nev3r",
                Email = "sallynvr1234@mail.com",
                FullName = "Sally Middle Never",
                CreationDate = DateTime.UtcNow,
                PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: "MyStrongPa$$word3",
                    Convert.FromBase64String(salts[2]),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: IterationCount,
                    numBytesRequested: HashByteSize)),
                Salt = salts[2],
                RoleId = 3
            },
            new User
            {
                Id = Guid.Parse("0190d1a3-439c-b416-92d3-6fd43e46ca0f"),
                Name = "mikenvr1345",
                Email = "mike.never@mail.com",
                FullName = "Michael Alex Never",
                CreationDate = DateTime.UtcNow,
                PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: "MyStrongPa$$word4",
                    Convert.FromBase64String(salts[3]),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: IterationCount,
                    numBytesRequested: HashByteSize)),
                Salt = salts[3],
                RoleId = 3
            },
            new User
            {
                Id = Guid.Parse("0190d1a3-439c-9fd9-941b-425cb494d8a4"),
                Name = "mrghost-annyhilator",
                Email = "alwsann@mail.com",
                FullName = "Ann Middle Always",
                CreationDate = DateTime.UtcNow,
                PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: "MyStrongPa$$word5",
                    Convert.FromBase64String(salts[4]),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: IterationCount,
                    numBytesRequested: HashByteSize)),
                Salt = salts[4],
                RoleId = 3
            }
        ]);
    }

    private void SeedPictures()
    {
        DbContext.Pictures.AddRange([
            new Picture
            {
                Id = Guid.Parse("019157fb-a9a9-8edd-8391-7464a9e815d4"),
                Created = DateTime.UtcNow,
                Title = "Test Picture 1",
                Width = 1280,
                Height = 768,
                PhysicalPath = "img/2024/08/16/019157fb-a9a9-8edd-8391-7464a9e815d4.jpg",
                UserId = Guid.Parse("0190d1a3-4389-98ff-816b-a83e4376af7f")
            },
            new Picture
            {
                Id = Guid.Parse("019157fb-a9bc-2b4e-8641-79ac3b5aa499"),
                Created = DateTime.UtcNow,
                Title = "Test Picture 2",
                Width = 1024,
                Height = 768,
                PhysicalPath = "img/2024/08/16/019157fb-a9bc-2b4e-8641-79ac3b5aa499.jpg",
                UserId = Guid.Parse("0190d1a3-439c-b416-92d3-6fd43e46ca0f")
            },
            new Picture
            {
                Id = Guid.Parse("019157fb-a9bc-47ee-a1be-5150efd2a39d"),
                Created = DateTime.UtcNow,
                Title = "Test Picture 3",
                Width = 3840,
                Height = 2160,
                PhysicalPath = "img/2024/08/16/019157fb-a9bc-47ee-a1be-5150efd2a39d.jpg",
                UserId = Guid.Parse("0190d1a3-439c-d68a-2636-7c9af6a8600b")
            }
            ]);
    }
}