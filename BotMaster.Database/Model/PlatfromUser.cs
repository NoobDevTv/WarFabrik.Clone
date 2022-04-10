using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BotMaster.Database.Model
{
    public class PlatformUser : IdEntity<int>
    {
        public string PlatformName { get; set; }

    }
}
