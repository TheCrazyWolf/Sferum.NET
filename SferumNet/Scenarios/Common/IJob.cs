namespace SferumNet.Scenarios.Common;

public interface IJob
{
    Task ExecuteAsync(bool run = true);
    bool CanBeExecuted();
    Task ProcessAsync();
}