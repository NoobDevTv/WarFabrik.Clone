using BotMaster.PluginSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotMaster.PluginSystem.Messages
{
    public class Message
    {
        public const int HeaderSize = sizeof(MessageType) + sizeof(int) + sizeof(int);
        public static Encoding Encoding { get; } = Encoding.UTF8;

        public MessageType Type { get; set; }

        /// <summary>
        /// Taget null or Empty = Broadcast
        /// 'Server id' => Server listens as Plugin
        ///
        /// </summary>
        public string TargetId { get; set; }

        public IReadOnlyList<byte> Data => data;

        private readonly byte[] data;

        public Message(MessageType type, byte[] data, string targetId = null)
        {
            Type = type;
            TargetId = targetId;
            this.data = data;
        }

        public Package ToPackage()
        {
            var targetId = TargetId;

            if (string.IsNullOrWhiteSpace(TargetId))
                targetId = "";

            var targetIdCount = Encoding.GetByteCount(targetId);
            var array = new byte[HeaderSize + targetIdCount + data.Length];
            var span = array.AsSpan();

            span[0] = (byte)Type;
            BitConverter.TryWriteBytes(span[sizeof(byte)..], targetIdCount);
            BitConverter.TryWriteBytes(span[(sizeof(byte) + sizeof(int))..], data.Length);

            Encoding.GetBytes(targetId, span[HeaderSize..]);
            data.CopyTo(span[(HeaderSize + targetIdCount)..]);

            return new(array);
        }

        public ReadOnlySpan<byte> DataAsSpan()
            => data;

        public static Message FromPackage(Package package)
        {
            var span = package.AsSpan();
            var type = (MessageType)span[0];
            var targetIdCount = BitConverter.ToInt32(span[sizeof(byte)..]);
            var dataLength = BitConverter.ToInt32(span[(sizeof(byte) + sizeof(int))..]);

            var targetSize = HeaderSize + targetIdCount;
            var target = Encoding.GetString(span[HeaderSize..targetSize]);
            var data = span[targetSize..(targetSize + dataLength)];

            return new Message(type, data.ToArray(), target);
        }

    }
}
