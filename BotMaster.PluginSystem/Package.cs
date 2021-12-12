using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.PluginSystem
{
    public readonly struct Package
    {
        public const int HeaderSize = sizeof(int) + sizeof(int);

        public readonly IReadOnlyList<byte> Content => content;

        public int ContractId { get; }
        public int Length { get; }

        private readonly byte[] content;

        public Package(int contractId, byte[] content)
        {
            ContractId = contractId;
            this.content = content;    
            Length = HeaderSize + content.Length;
        }
        public Package(int contractId, ReadOnlySpan<byte> buffer)
        {
            ContractId = contractId;
            content = buffer.ToArray();
            Length = HeaderSize + content.Length;
        }

        public int ToBytes(Span<byte> buffer)
        {
            BitConverter.TryWriteBytes(buffer[0..], ContractId);
            BitConverter.TryWriteBytes(buffer[sizeof(int)..], Content.Count);
            content.CopyTo(buffer[HeaderSize..]);
            return Length;
        }

        public ReadOnlySpan<byte> AsSpan()
            => content.AsSpan();
    }
}
