using Db.Interceptors;
using DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Db;
public static class ServiceExtension {
    public static void AddDataAccess(this IServiceCollection services, IConfiguration configuration) {
        var connection = configuration.GetConnectionString("Default") ??
            throw new ArgumentException("Connection string not found in config");

        services.AddDbContextPool<AppDbContext>(opt => {
            opt.EnableSensitiveDataLogging();
            opt.UseNpgsql(connection);
            // opt.AddInterceptors(new DateInterceptor());

            // TODO: https://github.com/dotnet/efcore/issues/35110#issuecomment-2488804270
            opt.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        services.AddSingleton<SeedIdentity>();
        // services.AddSingleton<DateInterceptor>();
    }
}

