using BotMaster.Database;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.RightsManagement;
public class UserConnectionContext : DatabaseContext
{
    public DbSet<UserConnection> UserConnections => Set<UserConnection>();
    public DbSet<PlattformUser> PlattformUsers => Set<PlattformUser>();
    public DbSet<User> Users => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var info = new FileInfo(Path.Combine(".", "additionalfiles", "Rights.db"));
        _ = optionsBuilder.UseSqlite($"Data Source={info.FullName}");
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable(x => x.ExcludeFromMigrations());
        modelBuilder.Entity<PlattformUser>().ToTable(x => x.ExcludeFromMigrations());

        modelBuilder.Entity("GroupPlattformUser").ToTable(x => x.ExcludeFromMigrations());
        modelBuilder.Entity("GroupRight").ToTable(x => x.ExcludeFromMigrations());
        modelBuilder.Entity("GroupUser").ToTable(x => x.ExcludeFromMigrations());
        modelBuilder.Entity("PlattformUserRight").ToTable(x => x.ExcludeFromMigrations());
        modelBuilder.Entity("RightUser").ToTable(x => x.ExcludeFromMigrations());
        modelBuilder.Entity<Group>().ToTable(x => x.ExcludeFromMigrations());
        modelBuilder.Entity<Right>().ToTable(x => x.ExcludeFromMigrations());
    }
}
