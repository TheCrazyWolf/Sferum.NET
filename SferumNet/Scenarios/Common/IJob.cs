namespace SferumNet.Scenarios.Common;

public interface IJob
{
    Task ExecuteAsync(bool isAlive);
    bool CanBeExecuted();
    Task ProcessAsync();
}