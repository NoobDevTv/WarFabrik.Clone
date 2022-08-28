namespace BotMaster.PluginSystem
{
    public class ProcessExitedException : Exception
    {
        public int ExitCode { get; set; }
        public ProcessExitedException(int exitCode)
        {
            ExitCode = exitCode;
        }
    }
}