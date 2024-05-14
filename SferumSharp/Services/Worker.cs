using SferumSharp.Models.Request;

namespace SferumSharp.Services;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly VkFactory _vkFactory;

    public Worker(ILogger<Worker> logger, VkFactory vkFactory)
    {
        _logger = logger;
        _vkFactory = vkFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        var accounts = await _vkFactory.GetAccounts();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!accounts.Any())
                accounts = await _vkFactory.GetAccounts();
            
            if(accounts.Last().expires <= DateTime.Now.Ticks)
                accounts = await _vkFactory.GetAccounts();

            var messageParams = new MessageParams()
            {
                PeerID = "2000000001",
                Token = accounts.Last().access_token,
                Message = "Добрый вечер"
            };

            await _vkFactory.MessageSend(messageParams);

            await Task.Delay(1000, stoppingToken);
        }
    }
}