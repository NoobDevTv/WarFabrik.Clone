

using Microsoft.EntityFrameworkCore;

using System.Runtime.Loader;

namespace BotMaster.Database
{
    public abstract class DatabaseContext : DbContext
    {
        //public DbSet<User> Users => Set<User>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            foreach (var type in AssemblyLoadContext
                                        .Default
                                        .Assemblies
                                        .Where(x => x.FullName.Contains(nameof(BotMaster)))
                                        .SelectMany(x => x.GetTypes())
                                        .Where(type => !type.IsAbstract && !type.IsInterface && typeof(Entity).IsAssignableFrom(type)))
            {
                if (modelBuilder.Model.FindEntityType(type) is null)
                    _ = modelBuilder.Model.AddEntityType(type);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
