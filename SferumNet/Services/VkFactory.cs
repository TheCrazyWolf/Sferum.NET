using Newtonsoft.Json;
using RestSharp;
using SferumNet.Services.Models;

namespace SferumNet.Services;

public class VkFactory
{
    private readonly RestClient _rest = new();

    public void Authorize(string _username, string _password)
    {
        var request = new RestRequest("https://login.vk.com/", Method.Post);
        request.AddParameter("act", "connect_authorize");

        var body = new
        {
            username = _username,
            password = _password,
            v = "5.207",
            oauth_force_hash = "0",
            is_registration = "0",
            oauth_response_type = "silent_token",
            is_oauth_migrated_flow = "0",
            save_user = "1",
            version = "1",
            app_id = "8223270"
        };

        request.AddBody(body);

        var result = _rest.Execute(request);
    }

    public async Task<IReadOnlyList<ResponceProfileWebToken>?> GetAccountsAsync(string remixdsid)
    {
        var request = new RestRequest("https://web.vk.me/?act=web_token&app_id=8202606");
        request.AddCookie("remixdsid", remixdsid, "/", "web.vk.me");
        var responce = await _rest.ExecuteAsync(request);

        try
        {
            return JsonConvert.DeserializeObject<IReadOnlyList<ResponceProfileWebToken>>(responce.Content);
        }
        catch (Exception e)
        {
            return null;
        }
    }
}