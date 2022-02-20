using BotMaster.Database;

using Microsoft.EntityFrameworkCore;

namespace BotMaster.Telegram.Database;
internal class TelegramContext : DatabaseContext
{
    public DbSet<TelegramUser> TelegramUsers => Set<TelegramUser>();

    public TelegramContext() : base()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var info = new FileInfo(Path.Combine(".", "additionalfiles", "Telegram.db"));
        _ = optionsBuilder.UseSqlite($"Data Source={info.FullName}");
        base.OnConfiguring(optionsBuilder);
    }

}
