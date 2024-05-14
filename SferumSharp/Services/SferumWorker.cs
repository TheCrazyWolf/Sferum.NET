using SferumSharp.Scenario;
using SferumSharp.Scenario.Base;

namespace SferumSharp.Services;

public class SferumWorker : BackgroundService
{
    private readonly ILogger<SferumWorker> _logger;
    private readonly VkFactory _vkFactory;

    private readonly IReadOnlyCollection<IScenario> _scenarios = new List<IScenario>
    {
        /*new WelcomeScenario(2000000001),
        new WelcomeScenario(2000000002),
        new WelcomeScenario(2000000042),
        new WelcomeScenario(2000000098),*/
        new WelcomeScenario(2000000132),
        new SpamStatsScenario(2000000132)
    };
    
    public SferumWorker(ILogger<SferumWorker> logger, VkFactory vkFactory) 
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

            foreach (var currentScenario in _scenarios)
            {
                _logger.LogInformation($"[{DateTimeOffset.Now}] Task {currentScenario.GetType().FullName} started");
                try
                {
                    await currentScenario.Handle(_vkFactory, accounts.Last());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    accounts = await _vkFactory.GetAccounts();
                }
                await Task.Delay(1200, stoppingToken);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}