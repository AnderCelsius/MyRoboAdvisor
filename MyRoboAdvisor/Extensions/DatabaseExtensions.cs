using Microsoft.EntityFrameworkCore;
using MyRoboAdvisor.Configuration;
using MyRoboAdvisor.Database.Data;

namespace MyRoboAdvisor.Extensions;

public static class DatabaseExtensions
{
  public static void AddDatabase<TDbContext>(
    this IServiceCollection serviceCollection,
    DatabaseConfiguration? databaseConfiguration)
    where TDbContext : DbContext
  {
    serviceCollection.AddDbContextFactory<TDbContext>(builder => builder.UseSqlite(databaseConfiguration.ConnectionString!));

    serviceCollection.AddTransient<DatabaseMigrator<TDbContext>>();
  }

  public static Task MigrateDatabaseToLatestVersion<TDbContext>(this IServiceProvider serviceProvider)
    where TDbContext : DbContext
  {
    return serviceProvider.GetRequiredService<DatabaseMigrator<TDbContext>>().MigrateDbToLatestVersion();
  }
}
