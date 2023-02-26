using BotMaster.Core.Configuration;
using BotMaster.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BotMaster.Commandos;
public class CommandosDbContext : DatabaseContext
{
    public DbSet<PersistentCommand> Commands => Set<PersistentCommand>();

    public CommandosDbContext()
    {
    }

    public CommandosDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if !MIGRATION
        var config = ConfigManager.GetConfiguration(Path.Combine("additionalfiles", "CommandosConfig.json")).GetSettings<CommandoConfiguration>();

        //var ilogger = NLog.LogManager.GetCurrentClassLogger();
        //ilogger.Debug("Commando Config: " + System.Text.Json.JsonSerializer.Serialize(config));
        DatabaseFactory.Initialize(config.DatabasePluginName);
        foreach (var item in DatabaseFactory.DatabaseConfigurators)
        {
            item.OnConfiguring(optionsBuilder, config.ConnectionString);
        }
#endif
        //var info = new FileInfo(config.ConnectionString);
        //_ = optionsBuilder.UseSqlite($"Data Source={info.FullName}");asd
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PersistentCommand>()
            .Property(x => x.Plattforms)
            .HasConversion(x => string.Join(',', x),
            x => x.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            new ValueComparer<List<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));
        base.OnModelCreating(modelBuilder);
    }
}
