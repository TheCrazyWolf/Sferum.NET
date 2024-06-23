using SferumNet.DbModels.Common;

namespace SferumNet.DbModels.Vk;

public class VkProfile : Entity
{
    public string Name { get; set; }
    public string RemixSid { get; set; }
    public long UserId { get; set; }
    public string AccessToken { get; set; }
    public long AccessTokenExpired { get; set; }
}