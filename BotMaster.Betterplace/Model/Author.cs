using System;
using System.Collections.Generic;
using System.Text;

namespace BotMaster.Betterplace.Model
{
    public class Author
    {
        public string Name { get; set; }
        public Picture Picture { get; set; }
        public Link[] Links { get; set; }
    }
}
