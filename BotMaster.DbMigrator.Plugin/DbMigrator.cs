using System;
using System.Reactive.Linq;

using BotMaster.Commandos;
using BotMaster.PluginSystem;
using BotMaster.RightsManagement;

using NLog;

namespace BotMaster.DbMigrator;
public class DbMigrator : Plugin
{
    public override IObservable<Package> Start(ILogger logger, IObservable<Package> receivedPackages)
    {
        logger.Info($"Migrating {nameof(RightsDbContext)}");
        using (var ctx = new RightsDbContext())
            ctx.Migrate();
        logger.Info($"Migrating {nameof(UserConnectionContext)}");
        using (var ctx = new UserConnectionContext())
            ctx.Migrate();
        logger.Info($"Migrating {nameof(CommandosDbContext)}");
        using (var ctx = new CommandosDbContext())
            ctx.Migrate();

        return Observable.Empty<Package>();
    }
}