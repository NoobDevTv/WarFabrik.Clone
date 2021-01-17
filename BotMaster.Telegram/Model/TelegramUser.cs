using BotMaster.Database;
using BotMaster.Database.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NoobDevBot.Telegram.Model
{
    public class TelegramUser : EntityExtension<User>
    {
        public long ChatId { get; set; }
       
    }
}