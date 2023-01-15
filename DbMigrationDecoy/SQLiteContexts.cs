using BotMaster.Commandos;
using BotMaster.RightsManagement;
using BotMaster.Telegram.Database;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbMigrationDecoy;
internal class SQLiteContexts
{

    //public class UserConnectionContextFactory : IDesignTimeDbContextFactory<UserConnectionContext>
    //{
    //    public UserConnectionContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<UserConnectionContext>();
    //        optionsBuilder.UseSqlite();

    //        return new UserConnectionContext(optionsBuilder.Options);
    //    }
    //}
    //public class RightsDbContextFactory : IDesignTimeDbContextFactory<RightsDbContext>
    //{
    //    public RightsDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<RightsDbContext>();
    //        optionsBuilder.UseSqlite();

    //        return new RightsDbContext(optionsBuilder.Options);
    //    }
    //}
    //public class CommandosDbContextFactory : IDesignTimeDbContextFactory<CommandosDbContext>
    //{
    //    public CommandosDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<CommandosDbContext>();
    //        optionsBuilder.UseSqlite();

    //        return new CommandosDbContext(optionsBuilder.Options);
    //    }
    //}
}
