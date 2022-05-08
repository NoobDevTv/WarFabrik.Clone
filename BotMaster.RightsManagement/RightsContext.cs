using BotMaster.Database;
using BotMaster.RightsManagement;

using Microsoft.EntityFrameworkCore;

namespace BotMaster.Telegram.Database;
public class RightsContext : DatabaseContext
{
    //public DbSet<TelegramUser> TelegramUsers => Set<TelegramUser>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<User> Users => Set<User>();
    public DbSet<PlattformUser> PlatformUsers => Set<PlattformUser>();
    public DbSet<Right> Rights => Set<Right>();

    public RightsContext() : base()
    {
    }


}
