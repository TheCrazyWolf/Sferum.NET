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

    public async Task<IReadOnlyList<AccountVkMe>> GetAccounts()
    {
        try
        {
            _remixdsid = UpdateRemixSid();

            var request = new RestRequest("https://web.vk.me/?act=web_token&app_id=8202606");
            request.AddCookie("remixdsid", _remixdsid, "/", "web.vk.me");

            var responce = await _restSharp.ExecuteAsync(request);
            
            return JsonConvert.DeserializeObject<IReadOnlyList<AccountVkMe>>(responce.Content ?? string.Empty) ??
                   new List<AccountVkMe>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        return new List<AccountVkMe>();
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