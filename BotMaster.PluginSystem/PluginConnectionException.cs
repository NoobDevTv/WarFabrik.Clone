namespace BotMaster.PluginSystem
{
    public class PluginConnectionException : Exception
    {
        public PluginConnectionException()
        {
        }

        public PluginConnectionException(string message) : base(message)
        {
        }
    }
}
