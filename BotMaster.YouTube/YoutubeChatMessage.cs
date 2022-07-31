using BotMaster.MessageContract;

namespace BotMaster.YouTube
{
    public record struct YoutubeChatMessage(ChatMessage ChatMessage, string ChannelId, DateTime SendTime);
}