namespace BotMaster.PluginSystem
{
    public readonly struct Package
    {
        public const int HeaderSize = (sizeof(int) * 4) + sizeof(int);

        public readonly IReadOnlyList<byte> Content => content;

        public Guid ContractId { get; }
        public int Length { get; }

        private readonly byte[] content;

        public Package(Guid contractId, byte[] content)
        {
            ContractId = contractId;
            this.content = content;    
            Length = HeaderSize + content.Length;
        }
        public Package(Guid contractId, ReadOnlySpan<byte> buffer)
        {
            ContractId = contractId;
            content = buffer.ToArray();
            Length = HeaderSize + content.Length;
        }

        public int ToBytes(Span<byte> buffer)
        {
            ContractId.TryWriteBytes(buffer);
            BitConverter.TryWriteBytes(buffer[(sizeof(int)*4)..], Content.Count);
            content.CopyTo(buffer[HeaderSize..]);
            return Length;
        }

        public ReadOnlySpan<byte> AsSpan()
            => content.AsSpan();

        public byte[] AsArray()
            => content;
    }
}
