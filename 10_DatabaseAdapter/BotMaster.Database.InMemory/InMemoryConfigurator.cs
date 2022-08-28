using Microsoft.EntityFrameworkCore;

namespace BotMaster.Database.SqLite;
public class InMemoryConfigurator : IDatabaseConfigurator
{
    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        _ = optionsBuilder.UseInMemoryDatabase(connectionString);
    }
}
