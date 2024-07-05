using SferumNet.DbModels.Enum;
using SferumNet.Scenarios.Common;
using SferumNet.Services;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Utils;

namespace SferumNet.Scenarios;

public class WelcomeScenario : BaseScenario
{
    public override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await base.ExecuteAsync(cancellationToken);
        
        while (!CancellationToken.IsCancellationRequested)
        {
            await UpdateProfileAndScAsync();
            
            await ResetCounterExecutedIfNextDayAsync();
            
            if(!CanBeExecuted())
                continue;
            
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

            var parametrs = new VkParameters
            {
                { "peer_id", _currentScDb.IdConversation },
                { "random_id", new Random().Next() },
                { "message", new Random().Next() }
            };

            var result = VkApi.Call("messages.send", parametrs);
            await ProccessInrecementExecutedAsync();
        }
        catch (Exception e)
        {
            
            if(e is UserAuthorizationFailException)
            {
                await RefreshIfTokenExpireAsync();
                return;
            }
            
            await Logger.LogAsync(IdScenario, EventType.Error, $"Ошибка при выполнении скрипта\n{e.Message}");
        }
    }

    public WelcomeScenario(SferumNetContext ef, DbLogger dbLogger, long idScenario) : base(ef, dbLogger, idScenario)
    {
    }
}