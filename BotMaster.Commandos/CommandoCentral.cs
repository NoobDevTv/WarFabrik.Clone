using BotMaster.MessageContract;

using Microsoft.EntityFrameworkCore;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BotMaster.Commandos;


//class CommandoAttribute : Attribute
//{
//    public CommandoAttribute(string commando, Func<bool, string> guard,  Action<string[]> action)
//    {

//    }
//}

/// <summary>
/// The Central for commands
/// </summary>
public class CommandoCentral
{
    readonly Subject<CommandMessage> messageSubject = new();

    public IReadOnlyCollection<PersistentCommand> Commands { get => _commands; }

    private readonly List<PersistentCommand> _commands = new List<PersistentCommand>();

    /// <summary>
    /// Initializes an instance for the commando central
    /// </summary>
    public CommandoCentral()
    {
    }

    /// <summary>
    /// Gets all persistent commans for a specific plattform, including shared commands
    /// </summary>
    /// <param name="plattform">Name of the plattform</param>
    /// <returns>List of persistent commands</returns>
    public static List<PersistentCommand> GetCommandsFor(string plattform)
    {
        using var ctx = new CommandosDbContext();
        using var trans = ctx.Database.BeginTransaction();
        ctx.Database.Migrate();
        trans.Commit();
        return ctx.Commands.ToList().Where(x => x.Plattforms.Count == 0 || x.Global || x.Plattforms.Contains(plattform)).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="commands">The observable that provide the command messages</param>
    /// <returns><paramref name="commands"/></returns>
    public IObservable<CommandMessage> CreateCommandStream(IObservable<CommandMessage> commands)
    {
        return commands.Do(messageSubject.OnNext);
    }

    /// <summary>
    /// Adds a command for potential exection based on <paramref name="guard"/>
    /// </summary>
    /// <param name="guard">if the <see cref="Func{CommandMessage, bool}"/>returns true, then <paramref name="action"/> will be executed</param>
    /// <param name="action">will be executed, if <paramref name="guard"/> returns true</param>
    /// <returns>The observable that should be subscribed, otherwise nothing will happen ¯\_(ツ)_/¯</returns>
    public virtual IDisposable AddCommand(Func<CommandMessage, bool> guard, Action<CommandMessage> action)
    {
        return messageSubject.Where(guard).Subscribe(action);
    }

    /// <summary>
    /// Adds a command for potential exection based on <paramref name="guard"/>
    /// </summary>
    /// <param name="action">will be executed, if <paramref name="commandNames"/> includes <see cref="CommandMessage.Command"/> (case sensitive)</param>
    /// <param name="commandNames">The different names for this command, will be used in a generic guard method</param>
    /// <returns>The observable that should be subscribed, otherwise nothing will happen ¯\_(ツ)_/¯</returns>
    public virtual IDisposable AddCommand(Action<CommandMessage> action, params PersistentCommand[] commandNames)
    {
        var commandStrs = _commands.Select(x => x.Command).ToList();
        commandNames = commandNames.Where(x => !commandStrs.Contains(x.Command)).ToArray();
        if (commandNames.Length == 0)
            return Disposable.Empty;
        _commands.AddRange(commandNames);
        return messageSubject.Where(c => commandNames.Any(x => x.Command == c.Command && x.Target == c.SourcePlattform)).Subscribe(action);
    }

    public virtual IDisposable AddCommand(Func<CommandMessage, bool> guard, Action<CommandMessage> action, params PersistentCommand[] commandNames)
    {
        var commandStrs = _commands.Select(x => x.Command).ToList();
        commandNames = commandNames.Where(x => !commandStrs.Contains(x.Command)).ToArray();
        if (commandNames.Length == 0)
            return Disposable.Empty;
        _commands.AddRange(commandNames);
        return messageSubject.Where(c => commandNames.Any(x => x.Command == c.Command && x.Target == c.SourcePlattform) && guard(c)).Subscribe(action);
    }


    //public IObservable AddCommand(CommandMessage commandMessage)
    //{
    //    commandStream.Where(x => commandMessage.Command == x.a).Do(action);
    //}
}
