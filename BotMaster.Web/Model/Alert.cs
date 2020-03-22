using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotMaster.Web.Model
{
    public sealed class Alert
    {
        public int Id { get; internal set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public int Amount { get; set; }
        public DateTime Created { get; internal set; }
    }
}
