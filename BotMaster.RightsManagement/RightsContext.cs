using BotMaster.Database;
using BotMaster.RightsManagement;

using Microsoft.EntityFrameworkCore;

namespace BotMaster.Telegram.Database;
public class RightsDbContext : DatabaseContext
{
    //public DbSet<TelegramUser> TelegramUsers => Set<TelegramUser>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<User> Users => Set<User>();
    public DbSet<PlattformUser> PlattformUsers => Set<PlattformUser>();
    public DbSet<Right> Rights => Set<Right>();

    public RightsDbContext() : base()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var info = new FileInfo(Path.Combine(".", "additionalfiles", "Rights.db"));
        _ = optionsBuilder.UseSqlite($"Data Source={info.FullName}");
        base.OnConfiguring(optionsBuilder);
    }

}
