
using BotMaster.Core;

using NonSucking.Framework.Serialization;

namespace BotMaster.Betterplace.MessageContract
{
    [Nooson]
    public partial class Donation
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 0;

        public int Id { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public int Donated_amount_in_cents { get; set; }
        public int Matched_amount_in_cents { get; set; }
        public bool Matched { get; set; }
        public string Score { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        public DateTime Confirmed_at { get; set; }
        public string[] Links { get; set; }

        public Donation()
        {
            
        }
        private Donation(int id, int typeId)
        {
            Id = id;
        }

    }
}
