namespace SferumNet.Services.Common;

public interface IScenarioConfigurator
{
    Task RunAsync();
    Task StopAsync();
    Task RestartAsync();
}