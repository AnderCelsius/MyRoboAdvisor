﻿using Microsoft.EntityFrameworkCore;

namespace MyRoboAdvisor.Database.Data;

public class DatabaseMigrator<TDbContext>
  where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _contextFactory;
    private readonly ILogger<DatabaseMigrator<TDbContext>> _logger;

    public DatabaseMigrator(IDbContextFactory<TDbContext> contextFactory, ILogger<DatabaseMigrator<TDbContext>> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task MigrateDbToLatestVersion()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Database.SetCommandTimeout(300);

        await using var dbConnection = context.Database.GetDbConnection();
        _logger.LogInformation(
          "Migrating standards database {DATABASE} on server {SERVER}...",
          dbConnection.Database,
          dbConnection.DataSource);

        await context.Database.MigrateAsync();

        _logger.LogInformation("Database migrated successfully");
    }
}
