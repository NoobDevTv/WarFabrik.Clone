using BotMaster.Database.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BotMaster.Database.MySql;
public class MySQLConfigurator : IDatabaseConfigurator
{
    public class MigrationContext : MigrationDatabaseContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseMySql("server=botdb;userid=botmaster;password=my_cool_secret;database=botmasterd", ServerVersion.Create(10, 9, 3, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb));
            base.OnConfiguring(optionsBuilder);
        }

    }

    public IAutoMigrationContextBuilder GetEmptyForMigration()
    {
        return new MigrationContext();
    }

    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        _ = optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}

