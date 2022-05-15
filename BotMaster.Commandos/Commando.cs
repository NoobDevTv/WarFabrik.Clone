﻿using BotMaster.MessageContract;

using NonSucking.Framework.Extension.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core;


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
    Subject<CommandMessage> messageSubject = new();

    public IReadOnlyCollection<string> Commands { get => _commands; }

    private List<string> _commands = new List<string>();

    /// <summary>
    /// Initializes an instance for the commando central
    /// </summary>
    public CommandoCentral()
    {
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
        _commands.AddRange(commandNames);
        return messageSubject.Where(c=> commandNames.IndexOf(c.Command) != -1).Subscribe(action);
    }

    public virtual IDisposable AddCommand(Func<CommandMessage, bool> guard, Action<CommandMessage> action,  params string[] commandNames)
    {
        _commands.AddRange(commandNames);
        return messageSubject.Where(c => commandNames.IndexOf(c.Command) != -1 && guard(c)).Subscribe(action);
    }


    //public IObservable AddCommand(CommandMessage commandMessage)
    //{
    //    commandStream.Where(x => commandMessage.Command == x.a).Do(action);
    //}
}
