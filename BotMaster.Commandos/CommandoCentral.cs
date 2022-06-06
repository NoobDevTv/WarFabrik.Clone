using BotMaster.MessageContract;

using Microsoft.EntityFrameworkCore;

using NonSucking.Framework.Extension.Collections;
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

    public IReadOnlyCollection<string> Commands { get => _commands; }

    private readonly List<string> _commands = new List<string>();

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
        ctx.Database.Migrate();
        return ctx.Commands.ToList().Where(x => x.Plattforms.Count == 0 || x.Plattforms.Contains(plattform)).ToList();
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
    public virtual IDisposable AddCommand(Action<CommandMessage> action, params string[] commandNames)
    {
        commandNames = commandNames.Except(_commands).ToArray();
        if (commandNames.Length == 0)
            return Disposable.Empty;
        _commands.AddRange(commandNames);
        return messageSubject.Where(c => commandNames.IndexOf(c.Command) != -1).Subscribe(action);
    }

    public virtual IDisposable AddCommand(Func<CommandMessage, bool> guard, Action<CommandMessage> action, params string[] commandNames)
    {
        commandNames = commandNames.Except(_commands).ToArray();
        if (commandNames.Length == 0)
            return Disposable.Empty;
        _commands.AddRange(commandNames);
        return messageSubject.Where(c => commandNames.IndexOf(c.Command) != -1 && guard(c)).Subscribe(action);
    }


    //public IObservable AddCommand(CommandMessage commandMessage)
    //{
    //    commandStream.Where(x => commandMessage.Command == x.a).Do(action);
    //}
}
