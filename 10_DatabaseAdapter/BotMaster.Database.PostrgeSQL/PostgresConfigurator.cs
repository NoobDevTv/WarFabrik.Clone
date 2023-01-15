﻿using BotMaster.Database.Migrations;

using Microsoft.EntityFrameworkCore;

namespace BotMaster.Database.SqLite;
public class PostgresConfigurator : IDatabaseConfigurator
{
    public class MigrationContext : MigrationDatabaseContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseNpgsql();
            base.OnConfiguring(optionsBuilder);
        }

    }

    public IAutoMigrationContextBuilder GetEmptyForMigration()
    {
        return new MigrationContext();
    }
    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        _ = optionsBuilder.UseNpgsql(connectionString);
    }
}
