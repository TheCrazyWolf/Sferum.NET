namespace SferumSharp.Models.Request;

public class MessageParams
{
    public long PeerID { get; set; }
    public string Token { get; set; }
    public string Message { get; set; }
}