
using NonSucking.Framework.Serialization;

namespace BotMaster.Betterplace.MessageContract
{
    [Nooson]
    public partial class Donation
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 0;

        public int Id { get; set; }
        [NoosonCustom(SerializeImplementationType = typeof(Donation), SerializeMethodName = nameof(DateTimeToBinary)
            , DeserializeImplementationType = typeof(Donation), DeserializeMethodName = nameof(DateTimeFromBinary))]
        public DateTime Created_at { get; set; }

        [NoosonCustom(SerializeImplementationType = typeof(Donation), SerializeMethodName = nameof(DateTimeToBinary)
            , DeserializeImplementationType = typeof(Donation), DeserializeMethodName = nameof(DateTimeFromBinary))]
        public DateTime Updated_at { get; set; }
        public int Donated_amount_in_cents { get; set; }
        public int Matched_amount_in_cents { get; set; }
        public bool Matched { get; set; }
        public string Score { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        [NoosonCustom(SerializeImplementationType = typeof(Donation), SerializeMethodName = nameof(DateTimeToBinary)
            , DeserializeImplementationType = typeof(Donation), DeserializeMethodName = nameof(DateTimeFromBinary))]
        public DateTime Confirmed_at { get; set; }
        public string[] Links { get; set; }

        private static void DateTimeToBinary(BinaryWriter bw, DateTime dt)
        {
            bw.Write(dt.ToBinary());
        }
        private static DateTime DateTimeFromBinary(BinaryReader br)
        {
            return DateTime.FromBinary(br.ReadInt64());
        }

        public Donation()
        {
        }

        private Donation(int id, int typeId)
        {
            Id = id;
        }

    }
}
