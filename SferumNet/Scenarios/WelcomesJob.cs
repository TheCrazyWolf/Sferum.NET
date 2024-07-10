using Microsoft.EntityFrameworkCore;
using SferumNet.Configs;
using SferumNet.Database;
using SferumNet.DbModels.Data;
using SferumNet.DbModels.Enum;
using SferumNet.Scenarios.Common;
using SferumNet.Services;
using VkNet.Exception;
using VkNet.Utils;

namespace SferumNet.Scenarios;

public class WelcomesJob : BaseJob
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
            await Task.Delay(CurrentJob?.Delay ?? ScConst.DelayDefault, cancellationToken);
        }
        
        await Logger.LogAsync(IdScenario, EventType.Info, "Сценарий завершен");
    }

    public override bool CanBeExecuted()
    {
        if (CurrentJob is null || CurrentProfileDb is null)
            return false;
        
        if (!(DateTime.Now.TimeOfDay >= CurrentJob.TimeStart && DateTime.Now.TimeOfDay <= CurrentJob.TimeEnd))
            return false;

        if (CurrentJob.TotalExecuted >= CurrentJob.MaxToExecute)
            return false;

        if (DateTime.Now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return CurrentJob.IsActiveForWeekend;
        
        return CurrentJob.IsActive;
    }

    public override async Task ProcessAsync()
    {
        if (CurrentJob is null || CurrentProfileDb is null)
            return;
        
        try
        {
            var parameters = new VkParameters
            {
                { "peer_id", CurrentJob.IdConversation },
                { "random_id", new Random().Next() },
                { "message", await GetSentencesAsync() }
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

    protected virtual async Task<string> GetSentencesAsync()
    {
        var countTotal = await Ef.WelcomeSentences.CountAsync();
        var randomIndex = new Random().Next(countTotal);
            
        var thisSentence = await Ef.WelcomeSentences
            .OrderBy(w => w.Id) 
            .Skip(randomIndex)
            .FirstOrDefaultAsync();

        return thisSentence is null ? $"База данных предложений не заполнена {new Random().Next()}" : thisSentence.Message;
    }

    public WelcomesJob(SferumNetContext ef, DbLogger dbLogger, long idScenario) : base(ef, dbLogger, idScenario)
    {
    }
}