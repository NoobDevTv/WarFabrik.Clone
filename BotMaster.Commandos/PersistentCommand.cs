using BotMaster.Database;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.Commandos;

[Table("PersistentCommands")]
public class PersistentCommand : IdEntity<int>, ICloneableGeneric<PersistentCommand>
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

    public PersistentCommand(string command, string text, string? target = null, bool secure = false, bool global = false)
    {
        Plattforms = new();
        Command = command;
        Text = text;
        Secure = secure;
        Target = target;
        Global = global;
    }
    public PersistentCommand(string command, string text, List<string> plattforms, string? target = null, bool secure = false, bool global = false) : this(command, text, target, secure, global)
    {
        Plattforms = plattforms;
    }

    public PersistentCommand Clone()
    {
        return new PersistentCommand(Command, Text, Plattforms, Target, Secure, Global) { Id = Id };
    }


    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PersistentCommand>()
            .Property(x => x.Plattforms)
            .HasConversion(x => string.Join(',', x),
            x => x.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            new ValueComparer<List<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));
    }
}
