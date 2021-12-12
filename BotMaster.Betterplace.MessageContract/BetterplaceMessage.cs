using BotMaster.PluginSystem.Messages;
using NonSucking.Framework.Extension.Rx.SumTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Betterplace.MessageContract
{
    class BetterplaceMessage : Variant<BetterplaceMessage.Donation, int>
    {
        public string TargetId { get; }

        public BetterplaceMessage(Donation donation, string targetId = null) : base(donation)
        {
            TargetId = targetId;
        }

        public record Donation(
            string Id, 
            DateTime Created_at, 
            DateTime Updated_at, 
            int Donated_amount_in_cents, 
            int Matched_amount_in_cents, 
            bool Matched, 
            string Score, 
            string Author,
            string Message,
            DateTime Confirmed_at,
            string[] Links)
        {
            public const int TypeId = 0;

            public byte[] ToArray()
            {
                var stringSize =
                    Encoding.UTF8.GetByteCount(Id) +
                    Encoding.UTF8.GetByteCount(Score) +
                    Encoding.UTF8.GetByteCount(Author) +
                    Encoding.UTF8.GetByteCount(Message) +
                    Links.Sum(link => Encoding.UTF8.GetByteCount(link));

                var dataSize = stringSize + sizeof(int) * 3 + sizeof(long) * 3 + sizeof(bool);
                var dataArray = new byte[dataSize];
                var data = dataArray.AsSpan();
                int index = 0;

                BitConverter.TryWriteBytes(data, TypeId);
                index += sizeof(int);

                BitConverter.TryWriteBytes(data[index..], Matched);
                index += sizeof(bool);

                BitConverter.TryWriteBytes(data[index..], Created_at.Ticks);
                index += sizeof(long);

                BitConverter.TryWriteBytes(data[index..], Updated_at.Ticks);
                index += sizeof(long);

                BitConverter.TryWriteBytes(data[index..], Confirmed_at.Ticks);
                index += sizeof(long);

                BitConverter.TryWriteBytes(data[index..], Donated_amount_in_cents);
                index += sizeof(long);

                BitConverter.TryWriteBytes(data[index..], Matched_amount_in_cents);
                index += sizeof(long);

                Encoding.UTF8.GetBytes(Id, data[index..]);
                Encoding.UTF8.GetBytes(Score, data[index..]);
                Encoding.UTF8.GetBytes(Author, data[index..]);
                Encoding.UTF8.GetBytes(Message, data[index..]);
                BitConverter.TryWriteBytes(data, Links.Length);

                for (int i = 0; i < Links.Length; i++)
                {
                    Encoding.UTF8.GetBytes(Links[i], data[index..]);
                }
                return dataArray;
            }

            public static Donation FromSpan(ReadOnlySpan<byte> data)
            {  
                var text = Encoding.UTF8.GetString(data);
                return new(text);
            }
        }

        public Message ToMessage()
            => Map(
                    textMessage => new Message(Contract.Id, MessageType.Defined, textMessage.ToArray(), TargetId),
                    number => new Message(Contract.Id, MessageType.Defined, null)
               );


        public static BetterplaceMessage CreateTextMessage(string text)
            => new Donation(text);

        public static BetterplaceMessage FromMessage(Message message)
        {
            if (message.Type != MessageType.Defined)
                throw new NotSupportedException("Custom messages are not supported by DefinedMessage");

            var data = message.DataAsSpan();
            var id = BitConverter.ToInt32(data[..1]);

            switch (id)
            {
                case Donation.TypeId:
                    return new(Donation.FromSpan(data[sizeof(int)..]), message.TargetId);
                default:
                    throw new NotSupportedException($"message {id} is a unknown message type in DefinedMessage");
            }
        }

        public static implicit operator BetterplaceMessage(Donation obj)
            => new(obj);
    }
}
