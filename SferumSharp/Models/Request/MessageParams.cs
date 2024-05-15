#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace SferumSharp.Models.Request;

public class MessageParams
{
    public long ChatId { get; set; }
    public string Token { get; set; }
    public string Message { get; set; }
}