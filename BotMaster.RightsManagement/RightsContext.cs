using BotMaster.Database;
using BotMaster.RightsManagement;

using Microsoft.EntityFrameworkCore;

namespace BotMaster.Telegram.Database;
public class RightsDbContext : BaseDatabaseContext
{
    //public DbSet<TelegramUser> TelegramUsers => Set<TelegramUser>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<User> Users => Set<User>();
    public DbSet<PlattformUser> PlattformUsers => Set<PlattformUser>();
    public DbSet<Right> Rights => Set<Right>();

    public RightsDbContext() : base()
    {
    }

    public RightsDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
