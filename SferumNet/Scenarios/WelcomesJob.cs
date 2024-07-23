using Microsoft.EntityFrameworkCore;
using SferumNet.Configs;
using SferumNet.DbModels.Enum;
using SferumNet.Scenarios.Common;
using VkNet.Exception;
using VkNet.Utils;

namespace SferumNet.Scenarios;

public class WelcomesJob : BaseJob
{
    public WelcomesJob(IServiceScopeFactory scopeFactory, long idScenario) : base(scopeFactory, idScenario)
    {
    }
    
    public override async Task ExecuteAsync(bool run = true)
    {
        await base.ExecuteAsync(run);

        while (IsRun)
        {
            await UpdateProfileAndScAsync();

            await ResetCounterExecutedIfNextDayAsync();

            if (!CanBeExecuted())
            {
                // Фикс нагрузки на процессор в простое
                Thread.Sleep(5000);
                continue;
            }

            await ProcessAsync();
            await Task.Delay(CurrentJob?.Delay ?? ScConst.DelayDefault);
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
            if (e is UserAuthorizationFailException)
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

        return thisSentence is null
            ? $"База данных предложений не заполнена {new Random().Next()}"
            : thisSentence.Message;
    }
    
}