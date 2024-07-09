namespace SferumNet.Scenarios.Common;

public interface IJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
    bool CanBeExecuted();
    Task ProcessAsync();
}