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
        public const int HeaderSize = sizeof(int);

        public readonly IReadOnlyList<byte> Content => content;

        public int Length => HeaderSize + content.Length;

        private readonly byte[] content;

        public Package(byte[] content)
        {
            this.content = content;
        }
        public Package(ReadOnlySpan<byte> buffer)
        {
            content = buffer.ToArray();
        }

        public int ToBytes(Span<byte> buffer)
        {
            BitConverter.TryWriteBytes(buffer[0..], Content.Count);
            content.CopyTo(buffer[HeaderSize..]);
            return Length;
        }

        public ReadOnlySpan<byte> AsSpan()
            => content.AsSpan();
    }
}
