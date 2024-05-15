using SamGK_Api.Interfaces.Client;
using SferumSharp.Scenario;
using SferumSharp.Scenario.Base;

namespace SferumSharp.Services;

public class SferumWorker : BackgroundService
{
    private readonly ILogger<SferumWorker> _logger;
    private readonly VkFactory _vkFactory;

    private readonly IReadOnlyCollection<IScenario> _scenarios = new List<IScenario>
    {
        /* Сценарии одноразового приветствия */
        new WelcomeScenario(2000000001), // Учительская
        new WelcomeScenario(2000000063), // Учительская (2)
        new WelcomeScenario(2000000002), // ИС-21-01
        new WelcomeScenario(2000000042), // ИС-23-01
        new WelcomeScenario(2000000098), // ИС-21-06
        new WelcomeScenario(2000000043), // ИС-23-02
        new WelcomeScenario(2000000132), // ИС-22-02 УП (тест)

        /* Сценарий отправки расписания */
        new SheduleScenario(2000000002, "254", SheduleSearchType.Group), // ИС-21-01
        new SheduleScenario(2000000042, "351", SheduleSearchType.Group), // ИС-23-01
        new SheduleScenario(2000000098, "259", SheduleSearchType.Group), // ИС-21-06
        new SheduleScenario(2000000043, "361", SheduleSearchType.Group), // ИС-23-02

        /* Сценарий спама */
        new SpamStatsScenario(2000000132) // ИС-22-02 УП (тест)
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

            if (accounts.Last().expires <= DateTime.Now.Ticks)
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
                    _logger.LogCritical(e.ToString());
                    accounts = await _vkFactory.GetAccounts();
                }

                await Task.Delay(1200, stoppingToken);
            }

            await Task.Delay(new Random().Next(3000, 15000), stoppingToken);
        }
    }
}