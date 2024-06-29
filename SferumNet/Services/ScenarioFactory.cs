using SferumNet.Scenarios;
using SferumNet.Scenarios.Common;
using SferumNet.Services.Common;

namespace SferumNet.Services;

public class ScenarioFactory : IScenarioFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ScenarioFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IScenario CreateScenario(string scenarioType, long id)
    {
        var ef = _serviceProvider.GetRequiredService<SferumNetContext>();
        var logger = _serviceProvider.GetRequiredService<DbLogger>();
        
        return scenarioType switch
        {
            "Welcome" => new WelcomeScenario(ef, logger, id),
            _ => throw new ArgumentException($"Неизвестный тип сценария: {scenarioType}")
        };
    }
}