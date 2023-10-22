using StreamingClient.Base.Model.OAuth;

using Twitch.Base;
using Twitch.Base.Services;
using Twitch.Base.Models.NewAPI.Users;
using Twitch.Base.Clients;
using System.Security.Cryptography;

namespace NewTwitchTestConsole;

internal class Program
{
    private record struct TwitchAuth(string ClientId, string ClientSecret, string OAuth, string RefreshToken);
    public static readonly List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
        {
           OAuthClientScopeEnum.channel_commercial,
            OAuthClientScopeEnum.channel_editor,
            OAuthClientScopeEnum.channel_read,
            OAuthClientScopeEnum.channel_subscriptions,

            OAuthClientScopeEnum.user_read,

            OAuthClientScopeEnum.bits__read,

            OAuthClientScopeEnum.channel__edit__commercial,

            OAuthClientScopeEnum.channel__manage__broadcast,
            OAuthClientScopeEnum.channel__manage__moderators,
            OAuthClientScopeEnum.channel__manage__polls,
            OAuthClientScopeEnum.channel__manage__predictions,
            OAuthClientScopeEnum.channel__manage__redemptions,
            OAuthClientScopeEnum.channel__manage__vips,

            OAuthClientScopeEnum.channel__moderate,

            OAuthClientScopeEnum.channel__read__editors,
            OAuthClientScopeEnum.channel__read__goals,
            OAuthClientScopeEnum.channel__read__hype_train,
            OAuthClientScopeEnum.channel__read__polls,
            OAuthClientScopeEnum.channel__read__predictions,
            OAuthClientScopeEnum.channel__read__redemptions,
            OAuthClientScopeEnum.channel__read__subscriptions,
            OAuthClientScopeEnum.channel__read__vips,

            OAuthClientScopeEnum.clips__edit,

            OAuthClientScopeEnum.chat__edit,
            OAuthClientScopeEnum.chat__read,

            OAuthClientScopeEnum.moderation__read,

            OAuthClientScopeEnum.moderator__read__chat_settings,
            OAuthClientScopeEnum.moderator__read__followers,

            OAuthClientScopeEnum.moderator__manage__banned_users,
            OAuthClientScopeEnum.moderator__manage__chat_messages,
            OAuthClientScopeEnum.moderator__manage__chat_settings,
            OAuthClientScopeEnum.moderator__manage__shoutouts,

            OAuthClientScopeEnum.user__edit,

            OAuthClientScopeEnum.user__manage__blocked_users,
            OAuthClientScopeEnum.user__manage__whispers,

            OAuthClientScopeEnum.user__read__blocked_users,

            OAuthClientScopeEnum.user__read__broadcast,
            OAuthClientScopeEnum.user__read__follows,
            OAuthClientScopeEnum.user__read__subscriptions,

            OAuthClientScopeEnum.whispers__read,
            OAuthClientScopeEnum.whispers__edit,
        };
    static async Task Main()
    {
        var tokenFile = System.Text.Json.JsonSerializer.Deserialize<TwitchAuth>(File.ReadAllText("Token.json"));
        //var connection2 = await TwitchConnection.ConnectViaAuthorizationCode(tokenFile.ClientId, tokenFile.ClientSecret, tokenFile.OAuth);

        //var oAuthModel = await connection2.OAuth.GetOAuthTokenModel(tokenFile.ClientId, tokenFile.OAuth, scopes);

        //var resposen = await TwitchConnection.ConnectViaOAuthToken(new (){accessToken = tokenFile.OAuth, clientID = tokenFile.ClientId, clientSecret = tokenFile.ClientSecret, refreshToken = tokenFile.RefreshToken });

        var resposen = await TwitchConnection.ConnectViaLocalhostOAuthBrowser(tokenFile.ClientId, tokenFile.ClientSecret, scopes, forceApprovalPrompt: false, oauthListenerURL: "http://localhost:8919/");
        //var tokenCopy = resposen.GetOAuthTokenCopy();

        //var test = await resposen.OAuth.GetOAuthTokenModel(tokenFile.ClientId, tokenFile.ClientSecret, tokenFile.OAuth, scopes);

        var api = resposen.NewAPI;
        var cr = await api.Users.GetCurrentUser();
        var chat = new ChatClient(resposen);

        chat.OnMessageReceived += Chat_OnMessageReceived;
        chat.OnWhisperMessageReceived += Chat_OnWhisperMessageReceived;
        await chat.Connect();
        
        var user = await api.Users.GetUserByLogin("NoobDevTv");
        await chat.Join(user); 
        await chat.SendWhisperMessage(cr, user, "Test Private");

        await api.Chat.SendWhisper(cr.id, user.id, "Test private via api");

        Console.WriteLine("We have done everything");
        Console.ReadLine();
        await chat.SendMessage(user, "Test");
        await api.Chat.SendWhisper(cr.id, user.id, "Test");
        var followers = await api.Channels.GetFollowers(cr);
        var follower = (await api.Channels.GetFollowers(user, 10)).ToList();

        //var api = connection2.NewAPI;
        //var user3 = await api.Users.GetCurrentUser();
        //var user = await api.Users.GetUserByLogin("NoobDevTv");
        //var user2 = await api.Users.GetUserByLogin("susch19");
        //var followers = await api.Channels.GetFollowerCount(user3);

        //var follower = (await api.Channels.GetFollowers(user3, 10)).ToList();

        //var noobDev = new Twitch.Base.Models.NewAPI.Users.UserModel { id = user.id };


        //var abc = await connection2.OAuth.GetOAuthTokenModel(tokenFile.ClientId, tokenFile.ClientSecret, tokenFile.OAuth, scopes, "http://localhost");

        //var browserConnectin = await TwitchConnection.ConnectViaAuthorizationCode(tokenFile.ClientId, tokenFile.ClientSecret, tokenFile.OAuth, redirectUrl: "http://localhost");
        //var copyBrowser = browserConnectin.GetOAuthTokenCopy();

        //var copy = connection2.GetOAuthTokenCopy();
        ////copy.authorizationCode = tokenFile.OAuth;
        ////var connection = await TwitchConnection.ConnectViaOAuthToken(new OAuthTokenModel { clientID = tokenFile.ClientId, clientSecret = tokenFile.ClientSecret, accessToken = copy.accessToken/*, authorizationCode = tokenFile.OAuth */});
        //var connection = await TwitchConnection.ConnectViaOAuthToken(copy);
        //var authTokenModel = await connection2.OAuth.GetOAuthTokenModel(tokenFile.ClientId, tokenFile.ClientSecret, copy.accessToken, scopes);

        //var api = connection.NewAPI;
        //var user = await api.Users.GetUserByLogin("NoobDevTv");
        //var noobDev = new Twitch.Base.Models.NewAPI.Users.UserModel { id = user.id };
        //var follower = await api.Channels.GetFollowers(noobDev, 10);
        //var count = await api.Channels.GetFollowerCount(noobDev);
    }

    private static void Chat_OnWhisperMessageReceived(object? sender, Twitch.Base.Models.Clients.Chat.ChatWhisperMessagePacketModel e)
    => Console.WriteLine($"Whisper: {e.Message}"  );

    private static void Chat_OnMessageReceived(object? sender, Twitch.Base.Models.Clients.Chat.ChatMessagePacketModel e)
    => Console.WriteLine(e.Message);
}
