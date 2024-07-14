using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PhotoManagerAPI.DataAccess;

namespace PhotoManagerAPI.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PhotoManagerDbContext>(optBuilder =>
            optBuilder.UseSqlServer(configuration.GetConnectionString("PhotoManager")));

        services.AddAutoMapper(config => config.AddMaps(AppDomain.CurrentDomain.GetAssemblies()));
        services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    }
}