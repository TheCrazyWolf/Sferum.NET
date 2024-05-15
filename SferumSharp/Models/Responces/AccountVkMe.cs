namespace SferumSharp.Models.Responces;

public class AccountVkMe
{
    public int user_id { get; set; }
    public int profile_type { get; set; }
    public string access_token { get; set; }
    public int expires { get; set; }
}