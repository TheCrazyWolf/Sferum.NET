using SferumSharp.Models.Request;
using SferumSharp.Models.Responces;
using SferumSharp.Scenario.Base;
using SferumSharp.Services;

namespace SferumSharp.Scenario;

public class WelcomeScenario : IScenario
{
    private DateTime _lastWelcomed = DateTime.MinValue;
    private readonly long _peerId;

    private readonly IList<string> _welcomeArray = new List<string>()
    {
        "Доброе утро", 
        "Всем доброе утро и хорошего дня", 
        "Доброе утро, желаю хорошего дня"
    };

    public WelcomeScenario(long chatId)
    {
        _peerId = chatId;
    }
    
    public async Task Handle(VkFactory vkFactory, ResponceAccount currentAccount)
    {
        if(DateTime.Now.Hour <= 9 && DateTime.Now.Hour >= 22)
            return;
        
        if(DateTime.Today == _lastWelcomed)
            return;

        var messageParams = new MessageParams
        {
            PeerID = _peerId,
            Message = _welcomeArray[new Random().Next(0, _welcomeArray.Count)],
            Token = currentAccount.access_token
        };
        
        await vkFactory.MessageSend(messageParams);

        _lastWelcomed = DateTime.Now;
    }
}