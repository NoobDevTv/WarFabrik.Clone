using System;
using System.Collections.Generic;
using System.Text;

namespace NoobDevBot.Telegram.DatabaseModels
{
    public class Group
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int RightId { get; set; }

        public List<NoobUser> Member { get; set; }
        
        public Group()
        {
            Member = new List<NoobUser>();
        }
    }
}
