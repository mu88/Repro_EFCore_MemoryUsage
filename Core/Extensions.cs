using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class Extensions
{
    public static void ConfigureDatabase(this IServiceCollection services, string connectionString, bool disablePooledDbContextFactory)
    {
        void Options(DbContextOptionsBuilder contextOptionsBuilder)
        {
            contextOptionsBuilder
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .AddInterceptors(new SkipLockedRowsQueryCommandInterceptor())
                .UseNpgsql(
                    connectionString,
                    sqlOptions => sqlOptions
                        .CommandTimeout(40)
                        .EnableRetryOnFailure(6, TimeSpan.FromSeconds(30), Array.Empty<string>()));
        }

        if (disablePooledDbContextFactory)
        {
            services.AddDbContextFactory<MyDbContext>(Options);
        }
        else
        {
            services.AddDbContextPool<MyDbContext>(Options);
            services.AddPooledDbContextFactory<MyDbContext>(Options);
        }
    }
}