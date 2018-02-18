using Microsoft.EntityFrameworkCore;
using NoobDevBot.Telegram.DatabaseModels;

namespace NoobDevBot.Telegram
{
    public class TelegramBotContext: DbContext
    {
        public DbSet<NoobUser> User { get; set; }
        public DbSet<Stream> Streams { get; set; }
        public DbSet<Right> Rights { get; set; }
        public DbSet<Group> Groups{ get; set; }
        public DbSet<GroupChat> GroupChat { get; set; }
        public DbSet<User_Right> User_Right { get; set; }
        public DbSet<Group_User> Group_User { get; set; }
        public DbSet<GroupChat_User> GroupChat_User { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=telegram.db");
        }
    }
}