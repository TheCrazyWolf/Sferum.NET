namespace SferumNet.Services.Common;

public interface IScenarioConfigurator
{
    public DateTime? DateTimeStarted { get; protected internal set; }
    Task RunAsync();
    Task StopAsync();
    Task RestartAsync();
}