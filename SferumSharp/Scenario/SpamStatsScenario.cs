using SferumSharp.Models.Request;
using SferumSharp.Models.Responces;
using SferumSharp.Scenario.Base;
using SferumSharp.Services;

namespace SferumSharp.Scenario;

public class SpamStatsScenario : IScenario
{
    private readonly long _chatId;
    
    private readonly IList<string> _welcomeArray = new List<string>()
    {
        "Доброе утро! \ud83d\ude0a Пусть сегодняшний день будет полон улыбок и радости! \ud83d\ude04", 
        "Доброе утро! \ud83c\udf1e Пусть каждый момент этого дня будет наполнен счастьем и теплом! \ud83d\udc96", 
        "Доброе утро! \u2600\ufe0f Начните свой день с позитива и энергии! \ud83d\udcaa", 
        "Доброе утро! \ud83d\ude03 Пусть сегодня будет день, когда исполняются мечты! \ud83c\udf1f", 
    };
    
    public SpamStatsScenario(long chatId)
    {
        _chatId = chatId;
    }
    
    public async Task Handle(VkFactory vkFactory, ResponceAccount currentAccount)
    {
        var messageParams = new MessageParams
        {
            PeerID = _chatId,
            Message = $"{ShuffleWords(GetRandomSentence())} => {Guid.NewGuid()}" ,
            Token = currentAccount.access_token
        };
        
        await vkFactory.MessageSend(messageParams);
        
    }
    
    private string GetRandomSentence()
    {
        Random random = new Random();
        int index = random.Next(0, _welcomeArray.Count);
        return _welcomeArray[index];
    }
        
    private string ShuffleWords(string sentence)
    {
        List<string> words = sentence.Split(' ').ToList();
        words = words.OrderBy(x => Guid.NewGuid()).ToList(); 
        return string.Join(" ", words);
    }
    
    
}