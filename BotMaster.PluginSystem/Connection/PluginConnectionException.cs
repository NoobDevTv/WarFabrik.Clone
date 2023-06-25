namespace BotMaster.PluginSystem.Connection
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
