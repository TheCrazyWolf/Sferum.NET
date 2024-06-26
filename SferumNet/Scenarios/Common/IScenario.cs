namespace SferumNet.Scenarios.Common;

public interface IScenario
{
    Task ExecuteAsync(CancellationToken cancellationToken);
    bool CanBeExecuted();
}