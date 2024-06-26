using SferumNet.DbModels.Common;
using SferumNet.DbModels.Vk;
using SferumNet.Services.Common;

namespace SferumNet.Services;

public class ScenarioConfigurator(SferumNetContext ef) : IScenarioConfigurator
{
    private CancellationToken _cancellationToken = new();
    
    private readonly SferumNetContext _ef = ef;

    public async Task RunAsync()
    {
        var profiles = await GetProfilesAsync();

        if (profiles is null)
            return;

        foreach (var profile in profiles)
        {
            var scenarios = await GetScenariosByProfileAsync(profile.Id);

            if(scenarios is null)
                continue;
            
            foreach (var sc in scenarios)
            {
                // TODO START
            }
        }
    }

    public Task StopAsync()
    {
        // TOOD FINISH
        return Task.CompletedTask;
    }

    public async Task RestartAsync()
    {
        await StopAsync();
        await RunAsync();
    }


    private async Task<ICollection<VkProfile>?> GetProfilesAsync()
    {
        return await _ef.VkProfiles.ToListAsync();
    }

    private async Task<ICollection<Scenario>?> GetScenariosByProfileAsync(long idProfile)
    {
        return await _ef.Scenarios.Where(sc => sc.IdProfile == idProfile)
            .Where(sc => sc.IsActive)
            .ToListAsync();
    }
}