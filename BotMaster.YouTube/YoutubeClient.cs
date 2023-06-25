using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace BotMaster.YouTube;
public class YoutubeClient
{

    public YouTubeService YouTubeService { get; init; }
    public LiveBroadcastSnippet? CurrentBroadcast { get; internal set; }

    public YoutubeClient(YouTubeService youTubeService)
    {
        YouTubeService = youTubeService;
    }

    public LiveChatMessage? SendMessage(string message)
    {
        if (CurrentBroadcast is null)
            return null;

        var liveChatMessage = new LiveChatMessage();
        var snippet = new LiveChatMessageSnippet();
        snippet.LiveChatId = CurrentBroadcast.LiveChatId;
        snippet.Type = "textMessageEvent";
        var detail = new LiveChatTextMessageDetails();

        snippet.TextMessageDetails = detail;
        liveChatMessage.Snippet = snippet;
        //liveChatMessage.Snippet = snippet;
        //{ 
        //    LiveChatId = CurrentBroadcast.LiveChatId, 
        //    Type = "textMessageEvent",
        //    //DisplayMessage = message,
        //    TextMessageDetails = new()
        //    {
        //        MessageText = message
        //    }
        //};
        var messages = message.Chunk(180).Select(x => new string(x)).ToList();
        for (int i = 0; i < messages.Count; i++)
        {
            string? item = messages[i];
            var msg = item;
            if (messages.Count > 1)
            {
                msg = $"({i + 1}/{messages.Count}) {item}";
            }
            detail.MessageText = msg;

            var insertReq = YouTubeService.LiveChatMessages.Insert(liveChatMessage, "snippet");
            var res = insertReq.Execute();
            if (i + 1 == messages.Count)
            {
                return res;
            }
        }
        return null;
    }
}
