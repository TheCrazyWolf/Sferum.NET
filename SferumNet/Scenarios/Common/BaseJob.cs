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

public class BaseJob : IJob, IDisposable
{
    protected readonly long IdScenario;
    protected CancellationToken CancellationToken;

    /* Services */
    private IServiceScope _scope;
    protected readonly DbLogger Logger;
    private readonly VkRemixFactory _vkRemixFactory;

    protected readonly SferumNetContext Ef;
    protected readonly VkApi VkApi;

    protected VkProfile? CurrentProfileDb;
    protected Job? CurrentJob;

    public BaseJob(IServiceScopeFactory scopeFactory, long idScenario)
    {
        _scope = scopeFactory.CreateScope();
        Ef = _scope.ServiceProvider.GetRequiredService<SferumNetContext>();
        Logger = _scope.ServiceProvider.GetRequiredService<DbLogger>();
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

    private async Task<Job?> GetScenarioAsync(long idSc)
    {
        return await Ef.Scenarios
            .FirstOrDefaultAsync(x => x.Id == idSc, cancellationToken: CancellationToken);
    }

    protected async Task UpdateProfileAndScAsync()
    {
        CurrentJob = await GetScenarioAsync(IdScenario);

        if (CurrentJob is null)
            return;

        CurrentProfileDb = await GetProfileAsync(CurrentJob.IdProfile);
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
        if (CurrentJob is null)
            return;

        if (CurrentJob.LastExecuted.Date != DateTime.Today.Date)
        {
            await Logger.LogAsync(IdScenario, EventType.Info, "День прошел. Сбрасываем счётчик");
            CurrentJob.TotalExecuted = 0;
            CurrentJob.LastExecuted = DateTime.Now;
            Ef.Update(CurrentJob);
            await Ef.SaveChangesAsync(CancellationToken);
        }
    }


    protected async Task ProccessIncrementExecutedAsync()
    {
        if (CurrentJob is null)
            return;

        CurrentJob.TotalExecuted++;
        CurrentJob.LastExecuted = DateTime.Now;
        Ef.Update(CurrentJob);
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

    public void Dispose()
    {
        Ef.Dispose();
        VkApi.Dispose();
        _scope.Dispose();
    }
}