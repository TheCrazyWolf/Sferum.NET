using Microsoft.EntityFrameworkCore;
using SferumNet.DbModels.Common;
using SferumNet.DbModels.Enum;
using SferumNet.DbModels.Vk;
using SferumNet.Services;
using VkNet;

namespace SferumNet.Scenarios.Common;

public class BaseSc : IScenario
{
    protected long _idScenario;
    protected CancellationToken _cancellationToken;
    
    /* Services */
    protected DbLogger _logger;
    private VkFactory _vkFactory;
    
    private SferumNetContext _ef;
    private VkApi _vkApi; 
    
    protected VkProfile? _currentProfileDb;
    protected virtual Scenario? _currentScDb { get; set; }
    
    public virtual Task ExecuteAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual bool CanBeExecuted()
    {
        throw new NotImplementedException();
    }

    public virtual Task ProcessAsync()
    {
        throw new NotImplementedException();
    }

    private async Task<VkProfile?> GetProfileAsync(long? idProfile)
    {
        return await _ef.VkProfiles.FirstOrDefaultAsync(x => x.Id == idProfile, cancellationToken: _cancellationToken);
    }

    private async Task<Scenario?> GetScenarioAsync(long idSc)
    {
        return await _ef.Scenarios.FirstOrDefaultAsync(x => x.Id == idSc, cancellationToken: _cancellationToken);
    }

    protected async Task UpdateProfileAndScAsync()
    {
        _currentScDb = await GetScenarioAsync(_idScenario);

        if (_currentScDb is null)
            return;
        
        _currentProfileDb = await GetProfileAsync(_currentScDb.IdProfile);
    }

    protected async Task RefreshIfTokenExpireAsync()
    {
        if (_currentProfileDb is null)
            return;
        
        if (_currentProfileDb.AccessTokenExpired <= DateTime.Now.Ticks)
        {
            await _logger.LogAsync(_idScenario, EventType.Info, "Срок действия токена истек. Запрашиваем новый");
            
            var accounts = await _vkFactory.GetAccountsAsync(_currentProfileDb.RemixSid);

            var webTokenAccount = accounts?.FirstOrDefault(x => x.UserId == _currentProfileDb.UserId);

            if (webTokenAccount is null)
            {
                await _logger.LogAsync(_idScenario, EventType.Error, "Не удалось обновить токен.");
                return;
            }

            _currentProfileDb.AccessTokenExpired = webTokenAccount.Expires;
            _currentProfileDb.UserId = webTokenAccount.UserId;
            _currentProfileDb.AccessToken = webTokenAccount.AccessToken;

            _ef.Update(_currentProfileDb);
            await _ef.SaveChangesAsync(_cancellationToken);
        }
    }

    protected async Task ResetCounterExecutedIfNextdayAsync()
    {
        if(_currentScDb is null)
            return;

        if (_currentScDb.LastExecuted.Date != DateTime.Today.Date)
        {
            await _logger.LogAsync(_idScenario, EventType.Info, "День прошел. Сбрасываем счётчик");
            _currentScDb.TotalExecuted = 0;
            _ef.Update(_currentScDb);
            await _ef.SaveChangesAsync(_cancellationToken);
        }
    }
}