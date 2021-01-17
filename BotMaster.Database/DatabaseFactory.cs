using System;
using System.Collections.Generic;
using System.Text;

namespace BotMaster.Database
{
    public abstract class DatabaseFactory
    {
        public abstract DatabaseContext GetDatabase(string source);
    }
}
