using Microsoft.EntityFrameworkCore;

namespace BotMaster.Database.SqLite;
public class MSSQLConfigurator : IDatabaseConfigurator
{
    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        _ = optionsBuilder.UseSqlServer(connectionString);
    }
}
