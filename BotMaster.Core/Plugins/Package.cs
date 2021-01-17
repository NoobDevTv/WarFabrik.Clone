using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core.Plugins
{
    public readonly struct Package
    {
        private const int headerSize = sizeof(PackageType) + sizeof(ulong) + sizeof(int);

        public readonly PackageType Type { get; }
        public readonly ulong Id { get; }
        public readonly IReadOnlyList<byte> Content => content;

        private readonly byte[] content;

        public Package(PackageType type, ulong id, byte[] content)
        {
            Type = type;
            Id = id;
            this.content = content;
        }

        public int ToBytes(Span<byte> buffer)
        {
            buffer[0] = (byte)Type;
            BitConverter.TryWriteBytes(buffer[1..], Id);
            BitConverter.TryWriteBytes(buffer[9..], Content.Count);
            content.CopyTo(buffer[13..]);
            return headerSize + Content.Count;
        }

        internal static Package FromMemory(Span<byte> data)
        {
            var type = (PackageType)data[0];
            var id = BitConverter.ToUInt64(data[1..]);
            var contentSize = BitConverter.ToInt32(data[9..]);
            var content = data[13..].ToArray();

            //auto package = reinterpret_cast<const Package*>(&data)[0];

            return new Package(type, id, content);
        }
    }
}
