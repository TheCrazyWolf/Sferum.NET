using SferumNet.DbModels.Enum;
using SferumNet.Scenarios.Common;
using SferumNet.Services;
using VkNet;
using VkNet.Model;

namespace SferumNet.Scenarios;

public class WelcomeScenario : BaseScenario
{
    public override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await base.ExecuteAsync(cancellationToken);
        
        await Logger.LogAsync(IdScenario, EventType.Info, "Сценарий запущен");
        
        while (!CancellationToken.IsCancellationRequested)
        {
            await UpdateProfileAndScAsync();
            
            if(!CanBeExecuted())
                continue;

            await RefreshIfTokenExpireAsync();
            await ResetCounterExecutedIfNextDayAsync();
            await ProcessAsync();
            await Task.Delay(_currentScDb?.Delay ?? 5000, cancellationToken);
        }
        
        await Logger.LogAsync(IdScenario, EventType.Info, "Сценарий завершен");
    }

    public override bool CanBeExecuted()
    {
        if (_currentScDb is null || _currentProfileDb is null)
            return false;
        
        // TODO: Проверка на время

        if (_currentScDb.TotalExecuted >= _currentScDb.MaxToExecute)
            return false;
        
        return _currentScDb.IsActive;
    }

    public override async Task ProcessAsync()
    {
        if (_currentScDb is null || _currentProfileDb is null)
            return;
        
        try
        {
            
            VkApi.Messages.Send(new MessagesSendParams
            {
                PeerId = _currentScDb.IdConversation,
                RandomId = new Random().Next()
            });
            
            await ProccessInrecementExecutedAsync();
        }
        catch (Exception e)
        {
            await Logger.LogAsync(IdScenario, EventType.Error, $"Ошибка при выполнении скрипта\n{e.Message}");
        }
    }

    public WelcomeScenario(SferumNetContext ef, DbLogger dbLogger, long idScenario) : base(ef, dbLogger, idScenario)
    {
    }
}