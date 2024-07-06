using Newtonsoft.Json;
using RestSharp;
using SferumNet.Services.Models;

namespace SferumNet.Services;

/// <summary>
/// Сервис по предоставлению профилей аккаунтов
/// используя REMIXDSID кук
/// </summary>
public class VkRemixFactory
{
    private readonly RestClient _rest = new();
    
    // Запилить бы еще кастомнуб авторизацию
    // быы....
    
    public async Task<IReadOnlyList<ResponceProfileWebToken>?> GetAccountsAsync(string remixdsid)
    {
        var request = new RestRequest("https://web.vk.me/?act=web_token&app_id=8202606");
        
        request.AddCookie("remixdsid", remixdsid, "/", "web.vk.me");
        
        var responce = await _rest.ExecuteAsync(request);

        return responce.Content is null ? null : TryDeserialize(responce.Content);
    }
    
    private IReadOnlyList<ResponceProfileWebToken>? TryDeserialize(string content)
    {
        try
        {
            return JsonConvert.DeserializeObject<IReadOnlyList<ResponceProfileWebToken>>(content);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return null;
        }
    }
}