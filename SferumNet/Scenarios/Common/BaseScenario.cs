using Microsoft.EntityFrameworkCore;
using SferumNet.Configs;
using SferumNet.Database;
using SferumNet.DbModels.Common;
using SferumNet.DbModels.Enum;
using SferumNet.DbModels.Vk;
using SferumNet.Services;
using VkNet;
using VkNet.Model;

namespace SferumNet.Scenarios.Common;

public class BaseScenario : IScenario
{
    protected readonly long IdScenario;
    protected CancellationToken CancellationToken;

    /* Services */
    protected readonly DbLogger Logger;
    private readonly VkRemixFactory _vkRemixFactory;

    protected readonly SferumNetContext Ef;
    protected readonly VkApi VkApi;

    protected VkProfile? CurrentProfileDb;
    protected Scenario? CurrentScDb;

    public BaseScenario(SferumNetContext ef, DbLogger dbLogger, long idScenario)
    {
        Ef = ef;
        Logger = dbLogger;
        IdScenario = idScenario;

        VkApi = new VkApi();
        _vkRemixFactory = new VkRemixFactory();
    }

    public virtual async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        await Logger.LogAsync(IdScenario, EventType.Info, "Сценарий запущен");
    }

    public virtual bool CanBeExecuted()
    {
        return false;
    }

    public virtual Task ProcessAsync()
    {
        return Task.CompletedTask;
    }

    private async Task<VkProfile?> GetProfileAsync(long? idProfile)
    {
        return await Ef.VkProfiles
            .FirstOrDefaultAsync(x => x.Id == idProfile, cancellationToken: CancellationToken);
    }

    private async Task<Scenario?> GetScenarioAsync(long idSc)
    {
        return await Ef.Scenarios
            .FirstOrDefaultAsync(x => x.Id == idSc, cancellationToken: CancellationToken);
    }

    protected async Task UpdateProfileAndScAsync()
    {
        CurrentScDb = await GetScenarioAsync(IdScenario);

        if (CurrentScDb is null)
            return;

        CurrentProfileDb = await GetProfileAsync(CurrentScDb.IdProfile);
        await ConfigureVkApiAsync();
    }

    protected async Task RefreshIfTokenExpireAsync()
    {
        if (CurrentProfileDb is null)
            return;

        await Logger.LogAsync(IdScenario, EventType.Info, "Срок действия токена истек. Запрашиваем новый");

        var accounts = await _vkRemixFactory.GetAccountsAsync(CurrentProfileDb.RemixSid);

        var webTokenAccount = accounts?.FirstOrDefault(x => x.UserId == CurrentProfileDb.UserId);

        if (webTokenAccount is null)
        {
            await Logger.LogAsync(IdScenario, EventType.Error, "Не удалось обновить токен.");
            return;
        }
        
        CurrentProfileDb.UserId = webTokenAccount.UserId;
        CurrentProfileDb.AccessToken = webTokenAccount.AccessToken;
        CurrentProfileDb.AccessTokenExpired = DateTime.Now.AddMinutes(10).Ticks;
        Ef.Update(CurrentProfileDb);
        await Ef.SaveChangesAsync(CancellationToken);
            
        await ConfigureVkApiAsync();
    }

    protected async Task ResetCounterExecutedIfNextDayAsync()
    {
        if (CurrentScDb is null)
            return;

        if (CurrentScDb.LastExecuted.Date != DateTime.Today.Date)
        {
            await Logger.LogAsync(IdScenario, EventType.Info, "День прошел. Сбрасываем счётчик");
            CurrentScDb.TotalExecuted = 0;
            Ef.Update(CurrentScDb);
            await Ef.SaveChangesAsync(CancellationToken);
        }
    }


    protected async Task ProccessIncrementExecutedAsync()
    {
        if (CurrentScDb is null)
            return;

        CurrentScDb.TotalExecuted++;
        CurrentScDb.LastExecuted = DateTime.Now;
        Ef.Update(CurrentScDb);
        await Ef.SaveChangesAsync(CancellationToken);

        await Logger.LogAsync(IdScenario, EventType.Success, $"Сценарий успешно выполнен");
    }

    private async Task ConfigureVkApiAsync()
    {
        VkApi.VkApiVersion.SetVersion(VkConst.VkApiMajor, VkConst.VkApiMinor);

        if (CurrentProfileDb is null)
            return;

        await VkApi.AuthorizeAsync(new ApiAuthParams
        {
            AccessToken = CurrentProfileDb.AccessToken
        }, CancellationToken);
    }
}