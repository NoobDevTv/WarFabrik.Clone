using BotMaster.Database;

namespace BotMaster.Commandos;

public class PersistentCommand : IdEntity<int>
{
    public string? Target { get; set; }
    public string Command { get; set; }
    public string Text { get; set; }
    public bool Secure { get; set; }

    public virtual List<string> Plattforms { get; set; }

    public PersistentCommand()
    {
        Command = Text = "";
        Secure = false;
        Plattforms = new();
    }

    public PersistentCommand(string command, string text, string target = "", bool secure = false)
    {
        Plattforms = new();
        Command = command;
        Text = text;
        Secure = secure;
        Target = target;
    }
    public PersistentCommand(string command, string text, List<string> plattforms, string target = "", bool secure = false) : this(command, text, target, secure)
    {
        Plattforms = plattforms;
    }
}
