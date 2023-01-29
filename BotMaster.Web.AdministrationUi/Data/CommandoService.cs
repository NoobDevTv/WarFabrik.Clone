using BotMaster.Commandos;
using BotMaster.Database;

using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.Web.AdministrationUi.Data;


public class CommandoService
{
    public PersistentCommand[] GetCommands()
    {
        using var sd = new CommandosDbContext();

        return sd.Commands.ToArray();
    }
}
