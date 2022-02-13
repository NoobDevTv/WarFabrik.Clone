using NonSucking.Framework.Serialization;

namespace BotMaster.MessageContract
{
    [Nooson]
    public partial class TextMessage
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
    }
}
