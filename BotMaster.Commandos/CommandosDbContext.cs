using BotMaster.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BotMaster.Commandos;
public class CommandosDbContext : DatabaseContext
{
    public DbSet<PersistentCommand> Commands => Set<PersistentCommand>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var info = new FileInfo(Path.Combine(".", "additionalfiles", "Rights.db"));
        _ = optionsBuilder.UseSqlite($"Data Source={info.FullName}");
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
