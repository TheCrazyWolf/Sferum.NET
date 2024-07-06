using Newtonsoft.Json;

namespace SferumNet.Services.Models;

public class ResponceProfileWebToken
{
    [JsonProperty("user_id")] public long UserId { get; set; }
    [JsonProperty("profile_type")] public int ProfileType { get; set; }
    [JsonProperty("access_token")] public string AccessToken { get; set; }
    [JsonProperty("expires")] public int Expires { get; set; }
}