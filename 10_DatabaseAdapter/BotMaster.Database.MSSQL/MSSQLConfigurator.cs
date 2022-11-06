using BotMaster.Database.Migrations;

using Microsoft.EntityFrameworkCore;

namespace BotMaster.Database.SqLite;
public class MSSQLConfigurator : IDatabaseConfigurator
{
    public class MigrationContext : MigrationDatabaseContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseSqlServer();
            base.OnConfiguring(optionsBuilder);
        }

    }

    public IAutoMigrationContextBuilder GetEmptyForMigration()
    {
        return new MigrationContext();
    }

    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        _ = optionsBuilder.UseSqlServer(connectionString);
    }
}
