using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PhotoManagerAPI.Core.Services;
using PhotoManagerAPI.DataAccess;
using PhotoManagerAPI.DataAccess.Repositories;

namespace PhotoManagerAPI.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PhotoManagerDbContext>(optBuilder =>
            optBuilder.UseSqlServer(configuration.GetConnectionString("PhotoManager")));

        services.AddAutoMapper(config => config.AddMaps(AppDomain.CurrentDomain.GetAssemblies()));
        services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        
        services.AddScoped<IUserService, UserService>();
    }

    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        if (!configuration.GetSection("Jwt").Exists())
            throw new NullReferenceException("No JWT settings provided!");
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            });
    }

    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhotoManagerAPI", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });
    }
}