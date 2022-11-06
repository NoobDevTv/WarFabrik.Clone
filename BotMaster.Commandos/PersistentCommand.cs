using BotMaster.Database;

using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.Commandos;

[Table("PersistentCommands")]
public class PersistentCommand : IdEntity<int>
{
    public string? Target { get; set; }
    public string Command { get; set; }
    public string Text { get; set; }
    public bool Secure { get; set; }
    public bool Global { get; set; }

    public virtual List<string> Plattforms { get; set; }

    public PersistentCommand()
    {
        Command = Text = "";
        Secure = false;
        Global = false;
        Plattforms = new();
    }

    public PersistentCommand(string command, string text, string target = "", bool secure = false, bool global = false)
    {
        Plattforms = new();
        Command = command;
        Text = text;
        Secure = secure;
        Target = target;
        Global = global;
    }
    public PersistentCommand(string command, string text, List<string> plattforms, string target = "", bool secure = false, bool global = false) : this(command, text, target, secure, global)
    {
        Plattforms = plattforms;
    }
}
