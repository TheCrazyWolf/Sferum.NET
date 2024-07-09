using SferumNet.Configs;
using SferumNet.DbModels.Enum;
using SferumNet.Scenarios.Common;
using SferumNet.Services;
using VkNet.Exception;
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
            await Task.Delay(CurrentScDb?.Delay ?? ScConst.DelayDefault, cancellationToken);
        }
        
        await Logger.LogAsync(IdScenario, EventType.Info, "Сценарий завершен");
    }

    public override bool CanBeExecuted()
    {
        if (CurrentScDb is null || CurrentProfileDb is null)
            return false;
        
        if (!(DateTime.Now.TimeOfDay >= CurrentScDb.TimeStart && DateTime.Now.TimeOfDay <= CurrentScDb.TimeEnd))
            return false;

        if (CurrentScDb.TotalExecuted >= CurrentScDb.MaxToExecute)
            return false;
        
        return CurrentScDb.IsActive;
    }

    public override async Task ProcessAsync()
    {
        if (CurrentScDb is null || CurrentProfileDb is null)
            return;
        
        try
        {
            var parameters = new VkParameters
            {
                { "peer_id", CurrentScDb.IdConversation },
                { "random_id", new Random().Next() },
                { "message", new Random().Next() }
            };

            VkApi.Call("messages.send", parameters);
            await ProccessIncrementExecutedAsync();
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