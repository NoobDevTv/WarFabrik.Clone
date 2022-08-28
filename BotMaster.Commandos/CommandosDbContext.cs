using BotMaster.Core.Configuration;
using BotMaster.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BotMaster.Commandos;
public class CommandosDbContext : DatabaseContext
{
    public DbSet<PersistentCommand> Commands => Set<PersistentCommand>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = ConfigManager.GetConfiguration(Path.Combine("additionalfiles", "CommandosConfig.json")).GetSettings<CommandoConfiguration>();

        DatabaseFactory.Initialize(config.DatabasePluginName);
        foreach (var item in DatabaseFactory.DatabaseConfigurators)
        {
            item.OnConfiguring(optionsBuilder, config.ConnectionString);
        }

        //var info = new FileInfo(config.ConnectionString);
        //_ = optionsBuilder.UseSqlite($"Data Source={info.FullName}");
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
