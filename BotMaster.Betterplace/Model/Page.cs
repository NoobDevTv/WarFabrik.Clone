using System;
using System.Collections.Generic;
using System.Text;

namespace BotMaster.Betterplace.Model
{
    public class Page
    {
        public int Total_entries { get; set; }
        public int Offset { get; set; }
        public int Total_pages { get; set; }
        public int Current_page { get; set; }
        public int Per_page { get; set; }
        public Opinion[] Data { get; set; }
    }
}
