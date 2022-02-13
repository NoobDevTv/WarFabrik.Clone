using BotMaster.Betterplace.MessageContract;

namespace BotMaster.Betterplace.Model
{
    public class Opinion
    {
        public int Id { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public int Donated_amount_in_cents { get; set; }
        public int Matched_amount_in_cents { get; set; }
        public bool Matched { get; set; }
        public string Score { get; set; }
        public Author Author { get; set; }
        public string Message { get; set; }
        public DateTime Confirmed_at { get; set; }
        public Link[] Links { get; set; }

        public Donation ToDonation()
        {
            return new Donation
            {
                Id = Id,
                Created_at = Created_at,
                Updated_at = Updated_at,
                Donated_amount_in_cents = Donated_amount_in_cents,
                Matched_amount_in_cents = Matched_amount_in_cents,
                Score = Score,
                Author = Author.Name,
                Message = Message,
                Confirmed_at = Confirmed_at,
                Links = Links.Select(x => x.Href).ToArray()
            };
        }
    }
}
