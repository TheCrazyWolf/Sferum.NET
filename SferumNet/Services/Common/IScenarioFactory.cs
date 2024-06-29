using SferumNet.Scenarios.Common;

namespace SferumNet.Services.Common;

public interface IScenarioFactory
{
    IScenario CreateScenario(string scenarioType, long id);
}