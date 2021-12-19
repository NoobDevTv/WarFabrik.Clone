namespace BotMaster.MessageContract
{
    public partial class TextMessage
    {
        public void Serialize(System.IO.BinaryWriter writer)
        {
            writer.Write(TypeId);
            writer.Write(Text);
        }

        public static TextMessage Deserialize(System.IO.BinaryReader reader)
        {
            var TypeId__ret__v = reader.ReadInt32();
            var Text__ret__w = reader.ReadString();
            var ret____ = new BotMaster.MessageContract.TextMessage(Text__ret__w, TypeId__ret__v);
            return ret____;
        }
    }
}