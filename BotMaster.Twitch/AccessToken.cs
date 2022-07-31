
using Newtonsoft.Json;

namespace BotMaster.Twitch
{
    public class AccessToken
    {
        [JsonProperty(PropertyName = "access_token")]
        public string Token { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public double ExpiresIn { get; set; }
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public bool IsExpired => CreatedAt.AddSeconds(ExpiresIn) <= DateTime.Now;

    }
}
