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
        "Доброе утро! \ud83d\ude0a Пусть сегодняшний день будет полон улыбок и радости! \ud83d\ude04", 
        "Доброе утро! \ud83c\udf1e Пусть каждый момент этого дня будет наполнен счастьем и теплом! \ud83d\udc96", 
        "Доброе утро! \u2600\ufe0f Начните свой день с позитива и энергии! \ud83d\udcaa", 
        "Доброе утро! \ud83d\ude03 Пусть сегодня будет день, когда исполняются мечты! \ud83c\udf1f", 
        "Доброе утро! \ud83c\udf3c Пусть этот день будет началом новых приключений и успехов! \ud83d\ude80", 
        "Доброе утро! \ud83d\ude0a Пусть сегодняшний день принесет вам много улыбок и радости! \ud83d\ude04", 
        "Доброе утро! \ud83c\udf1e Пусть каждый ваш день начинается с благополучия и счастья! \ud83d\udc96", 
        "Доброе утро! \ud83d\ude04 Пусть этот день будет наполнен позитивом и любовью! \u2764\ufe0f", 
        "Доброе утро! \u2600\ufe0f Пусть сегодняшний день станет лучшим днем вашей жизни! \ud83c\udf08", 
        "Доброе утро! \ud83d\ude0a Пусть этот день будет полон радости и улыбок! \ud83d\ude04", 
        "Доброе утро! \ud83c\udf1e Пусть каждый ваш день начинается с нежности и заботы! \ud83d\udc95", 
        "Доброе утро! \ud83d\ude03 Пусть сегодняшний день принесет вам море вдохновения и творчества! \ud83c\udfa8", 
        "Доброе утро! \ud83c\udf3c Пусть этот день будет началом новых возможностей и успехов! \ud83c\udf1f", 
        "Доброе утро! \ud83d\ude0a Пусть ваш день начнется с улыбки и хорошего настроения! \ud83d\ude04", 
        "Доброе утро! \ud83c\udf1e Пусть этот день будет полон радости и позитива! \u2728", 
        "Доброе утро! \ud83d\ude04 Пусть сегодняшний день принесет вам море удачи и улыбок! \ud83d\ude0a", 
        "Доброе утро! \ud83c\udf3c Пусть этот день будет началом ваших самых заветных мечтаний! \ud83d\udcad", 
        "Доброе утро! \ud83d\ude0a Пусть каждый момент этого дня будет наполнен счастьем и любовью! \u2764\ufe0f", 
        "Доброе утро! \ud83c\udf1e Пусть сегодняшний день будет полон приятных сюрпризов и радости! \ud83c\udf89", 
        "Доброе утро! \ud83d\ude04 Пусть этот день станет началом вашего нового успеха! \ud83d\ude80", 
        "Доброе утро! \ud83c\udf3c Пусть сегодняшний день принесет вам много вдохновения и энергии! \ud83d\udcaa", 
        "Доброе утро! \ud83d\ude0a Пусть этот день будет полон света и тепла! \u2600\ufe0f", 
        "Доброе утро! \ud83c\udf1e Пусть каждый момент этого дня будет наполнен счастьем и радостью! \ud83d\ude04", 
        "Доброе утро! \ud83d\ude03 Пусть этот день будет началом вашей невероятной истории успеха! \ud83c\udf1f", 
        "Доброе утро! \ud83c\udf3c Пусть сегодняшний день принесет вам много позитива и улыбок! \ud83d\ude0a", 
        "Доброе утро! \ud83d\ude0a Пусть ваш день начнется с лучших пожеланий и заботы! \ud83d\udc96", 
        "Доброе утро! \ud83c\udf1e Пусть сегодняшний день будет днем волшебства и счастья! \u2728", 
        "Доброе утро! \ud83d\ude04 Пусть этот день станет началом ваших самых заветных мечтаний! \ud83d\udcad", 
        "Доброе утро! \ud83c\udf3c Пусть сегодняшний день принесет вам много радости и позитива! \ud83d\ude0a", 

    };

    public WelcomeScenario(long chatId)
    {
        _peerId = chatId;
    }
    
    public async Task Handle(VkFactory vkFactory, ResponceAccount currentAccount)
    {
        if(DateTime.Today == _lastWelcomed.Date)
            return;
        
        if(DateTime.Now.Hour <= 9 && DateTime.Now.Hour >= 22)
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