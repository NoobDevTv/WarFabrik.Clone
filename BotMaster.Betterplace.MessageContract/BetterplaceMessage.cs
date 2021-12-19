using BotMaster.PluginSystem.Messages;
using dotVariant;
using NonSucking.Framework.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Betterplace.MessageContract
{
    [Variant]
    public partial class BetterplaceMessage
    {
        static partial void VariantOf(Donation donation);

        public string TargetId { get; }

        public BetterplaceMessage(Donation donation, string targetId) : this(donation)
        {
            TargetId = targetId;
        }

        public Message ToMessage()
        {
            using var memory = new MemoryStream();
            using var writer = new BinaryWriter(memory);
            return Visit(
                        donation =>
                        {
                            donation.Serialize(writer);
                            return new Message(Contract.Id, MessageType.Defined, memory.ToArray(), TargetId);
                        }
            );
        }


        public static BetterplaceMessage FromMessage(Message message)
        {
            if (message.Type != MessageType.Defined)
                throw new NotSupportedException("Custom messages are not supported by DefinedMessage");

            var data = message.DataAsSpan();
            var id = BitConverter.ToInt32(data[..1]);

            using var memory = new MemoryStream(data.ToArray());
            using var binaryReader = new BinaryReader(memory);

            return id switch
            {
                Donation.TypeId => new(Donation.Deserialize(binaryReader), message.TargetId),
                _ => throw new NotSupportedException($"message {id} is a unknown message type in DefinedMessage"),
            };
        }

    }

    [Nooson]
    public partial class Donation
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 0;

        public string Id { get; set; }
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

        public Donation(string id, int typeId = 0)
        {

        }

    }
}
