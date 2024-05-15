using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace SferumSharp.Models.Responces;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class AccountVkMe
{
    public int user_id { get; set; }
    public int profile_type { get; set; }
    public string access_token { get; set; }
    public int expires { get; set; }
}