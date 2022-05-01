using BotMaster.Database;
using BotMaster.RightsManagement;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.Telegram.Database;
public class TelegramUser : Entity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    public virtual User User { get; set; }
}
