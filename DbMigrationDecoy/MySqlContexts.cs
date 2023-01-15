using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotMaster.RightsManagement;
using BotMaster.Telegram.Database;
using BotMaster.Commandos;

namespace DbMigrationDecoy;
internal class MySqlContexts
{


    public class UserConnectionContextFactory : IDesignTimeDbContextFactory<UserConnectionContext>
    {
        public UserConnectionContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserConnectionContext>();
            optionsBuilder.UseMySql(ServerVersion.Create(10, 9, 3, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb), b => b.MigrationsAssembly("DbMigrationDecoy"));

            return new UserConnectionContext(optionsBuilder.Options);
        }
    }
    public class RightsDbContextFactory : IDesignTimeDbContextFactory<RightsDbContext>
    {
        public RightsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RightsDbContext>();
            optionsBuilder.UseMySql(ServerVersion.Create(10, 9, 3, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb), b => b.MigrationsAssembly("BotMaster.Database.MySql"));

            return new RightsDbContext(optionsBuilder.Options);
        }
    }
    public class CommandosDbContextFactory : IDesignTimeDbContextFactory<CommandosDbContext>
    {
        public CommandosDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CommandosDbContext>();
            optionsBuilder.UseMySql(ServerVersion.Create(10, 9, 3, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb), b => b.MigrationsAssembly("BotMaster.Database.MySql"));

            return new CommandosDbContext(optionsBuilder.Options);
        }
    }
}
