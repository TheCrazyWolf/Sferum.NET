using SferumNet.DbModels.Common;

namespace SferumNet.DbModels.Vk;

public class VkProfile : Entity
{
    public string Name { get; set; } = string.Empty;
    public string RemixSid { get; set; } = string.Empty;
    public long UserId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public long AccessTokenExpired { get; set; }
}