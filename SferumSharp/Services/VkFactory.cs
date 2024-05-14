using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using SferumSharp.Models.Request;
using SferumSharp.Models.Responces;

namespace SferumSharp.Services;

public class VkFactory(IConfiguration configuration)
{
    private readonly RestClient _restSharp = new RestClient();

    private string _remixdsid = string.Empty;

    public async Task<IReadOnlyList<ResponceAccount>> GetAccounts()
    {
        _remixdsid = UpdateRemixSid();

        var request = new RestRequest("https://web.vk.me/?act=web_token&app_id=8202606");
        request.AddCookie("remixdsid", _remixdsid, "/", "web.vk.me");

        var responce = await _restSharp.ExecuteAsync(request);

        if (!responce.IsSuccessStatusCode)
            throw new Exception("Failed to getAccounts");

        return JsonConvert.DeserializeObject<IReadOnlyList<ResponceAccount>>(responce.Content ?? string.Empty) ??
               new List<ResponceAccount>();
    }

    public async Task MessageSend(MessageParams messageParams)
    {
        var request = new RestRequest($"https://api.vk.com/method/messages.send");

        request.AddParameter("access_token", messageParams.Token);
        request.AddParameter("peer_id", messageParams.PeerID);
        request.AddParameter("message", messageParams.Message);
        request.AddParameter("random_id", new Random().Next());
        request.AddParameter("v", "5.226");
        
        var responce = await _restSharp.ExecuteAsync(request);
    }

    private string UpdateRemixSid()
    {
        return string.IsNullOrEmpty(_remixdsid)
            ? configuration.GetValue<string>("remixdsid") ?? string.Empty
            : _remixdsid;
    }
}