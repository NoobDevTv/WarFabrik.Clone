using BotMaster.Database;
using BotMaster.RightsManagement;

using Microsoft.EntityFrameworkCore;

namespace BotMaster.Telegram.Database;
internal class TelegramDBContext : RightsContext
{
    //public DbSet<TelegramUser> TelegramUsers => Set<TelegramUser>();

    public TelegramDBContext() : base()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var info = new FileInfo(Path.Combine(".", "additionalfiles", "Telegram.db"));
        _ = optionsBuilder.UseSqlite($"Data Source={info.FullName}");
        base.OnConfiguring(optionsBuilder);
    }
}
