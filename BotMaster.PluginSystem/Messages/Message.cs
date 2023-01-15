using System.Text;

using NonSucking.Framework.Serialization;

namespace BotMaster.PluginSystem.Messages
{
    [Nooson]
    public partial class Message
    {
        public static Message Empty = new Message(Guid.Empty, MessageType.None, Array.Empty<byte>());

        public const int HeaderSize = sizeof(MessageType) + sizeof(int) + sizeof(int);
        [NoosonIgnore]
        public static Encoding Encoding { get; } = Encoding.UTF8;
        [NoosonIgnore]
        public static int NextId { get => nextId++; }
        private static int nextId = 0;

        private int id = -1;

        public int Id
        {
            get {
                if (id == -1)
                    id = NextId;
                return id;
            } set => id = value;
        }

        public MessageType Type { get; }

        /// <summary>
        /// Taget null or Empty = Broadcast
        /// 'Server id' => Server listens as Plugin
        ///
        /// </summary>
        public string TargetId { get; }

        public Guid ContractUID { get; }

        [NoosonIgnore]
        public IReadOnlyList<byte> Data => data;


        [NoosonInclude]
        private readonly byte[] data;

        public Message(Guid contractUid, MessageType type, byte[] data, string targetId = null)
        {
            ContractUID = contractUid;
            Type = type;
            TargetId = targetId;
            this.data = data;
        }

        public Package ToPackage()
        {
            //var targetId = TargetId;
            //var sourceId = SourceId;

            //if (string.IsNullOrWhiteSpace(targetId))
            //    targetId = "";
            //if (string.IsNullOrWhiteSpace(sourceId))
            //    sourceId = "";

            //var targetIdCount = Encoding.GetByteCount(targetId);
            //var sourceIdCount = Encoding.GetByteCount(sourceId);
            //var array = new byte[HeaderSize + targetIdCount + sourceIdCount + data.Length];
            //var span = array.AsSpan();

            //span[0] = (byte)Type;
            //BitConverter.TryWriteBytes(span[sizeof(byte)..], targetIdCount);
            //BitConverter.TryWriteBytes(span[(sizeof(byte) + sizeof(int))..], sourceIdCount);
            //BitConverter.TryWriteBytes(span[(sizeof(byte) + sizeof(int) + sizeof(int))..], data.Length);

            //Encoding.GetBytes(targetId, span[HeaderSize..]);
            //data.CopyTo(span[(HeaderSize + targetIdCount)..]);
            //Encoding.GetBytes(sourceId, span[HeaderSize..]);
            //data.CopyTo(span[(HeaderSize + targetIdCount + sourceIdCount)..]);

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            Serialize(bw);
            bw.Flush();
            return new(ContractUID, ms.ToArray());
        }

        public ReadOnlySpan<byte> DataAsSpan()
            => data;

        public static Message FromPackage(Package package)
        {
            using var ms = new MemoryStream(package.AsArray());
            using var br = new BinaryReader(ms);
            return Deserialize(br);

            //var span = package.AsSpan();
            //var type = (MessageType)span[0];
            //var targetIdCount = BitConverter.ToInt32(span[sizeof(byte)..]);
            //var dataLength = BitConverter.ToInt32(span[(sizeof(byte) + sizeof(int))..]);

            //var targetSize = HeaderSize + targetIdCount;
            //var target = Encoding.GetString(span[HeaderSize..targetSize]);
            //var data = span[targetSize..(targetSize + dataLength)];

            //return new Message(package.ContractId, type, data.ToArray(), target);
        }

    }
}
