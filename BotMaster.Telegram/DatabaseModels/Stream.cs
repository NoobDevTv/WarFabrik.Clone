using System;

namespace NoobDevBot.Telegram.DatabaseModels
{
    public class Stream
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public DateTime Start { get; set; }

        public int UserId { get; set; }
    }
}