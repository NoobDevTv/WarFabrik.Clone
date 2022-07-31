namespace BotMaster.Database
{
    public abstract class DatabaseFactory
    {
        public abstract DatabaseContext GetDatabase(string source);
    }
}
