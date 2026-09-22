using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WebApp.Data
{
    public class JwtToken
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; } = string.Empty;

        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }


    }
}
