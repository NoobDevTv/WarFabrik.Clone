using NonSucking.Framework.Serialization;

namespace BotMaster.MessageContract
{
    [Nooson]
    public partial struct TextMessage : IEquatable<TextMessage>
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 0;

        public string Text { get; set; }

        public TextMessage(string text)
        {
            Text = text;
        }

        private TextMessage(string text, int typeId)
        {
            Text = text;
        }

        public override bool Equals(object obj) => obj is TextMessage message && Equals(message);
        public bool Equals(TextMessage other) => Text == other.Text;
        public override int GetHashCode() => HashCode.Combine(Text);

        public static bool operator ==(TextMessage left, TextMessage right) => left.Equals(right);
        public static bool operator !=(TextMessage left, TextMessage right) => !(left == right);
    }
}
