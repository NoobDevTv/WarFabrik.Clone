using NonSucking.Framework.Serialization;

namespace BotMaster.MessageContract
{
    [Nooson]
    public partial struct ChatMessage : IEquatable<ChatMessage>
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 2;

        public string Username { get; set; }
        public string Text { get; set; }

        public ChatMessage(string username, string text)
        {
            Username = username;
            Text = text;
        }

        private ChatMessage(string username, string text, int typeId)
        {
            Username = username;
            Text = text;
        }

        public override bool Equals(object obj) => obj is ChatMessage message && Equals(message);
        public bool Equals(ChatMessage other) => Username == other.Username && Text == other.Text;
        public override int GetHashCode() => HashCode.Combine(Username, Text);

        public static bool operator ==(ChatMessage left, ChatMessage right) => left.Equals(right);
        public static bool operator !=(ChatMessage left, ChatMessage right) => !(left == right);
    }
}
