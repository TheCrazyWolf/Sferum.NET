
using SferumNet.DbModels.Enum;
using SferumNet.Scenarios.Common;

namespace SferumNet.Scenarios;

public class WelcomeSc : BaseSc
{
    public override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _logger.LogAsync(_idScenario, EventType.Info, "Сценарий запущен");
        
        while (!_cancellationToken.IsCancellationRequested)
        {
            await UpdateProfileAndScAsync();
            
            if(!CanBeExecuted())
                continue;

            await RefreshIfTokenExpireAsync();
            await ResetCounterExecutedIfNextdayAsync();
            await ProcessAsync();
            await Task.Delay(_currentScDb?.Delay ?? 5000, cancellationToken);
        }
        
        await _logger.LogAsync(_idScenario, EventType.Info, "Сценарий завершен");
    }

    public override bool CanBeExecuted()
    {
        if (_currentScDb is null || _currentProfileDb is null)
            return false;
        
        // TODO: Проверка на время
        
        return _currentScDb.IsActive;
    }

    public override async Task ProcessAsync()
    {
        try
        {
            // TODO
        }
        catch (Exception e)
        {
            await _logger.LogAsync(_idScenario, EventType.Error, $"Ошибка при выполнении скрипта\n{e.Message}");
        }
    }
}