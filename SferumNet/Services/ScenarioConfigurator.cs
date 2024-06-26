using Microsoft.EntityFrameworkCore;
using SferumNet.DbModels.Common;
using SferumNet.DbModels.Vk;
using SferumNet.Scenarios;
using SferumNet.Services.Common;

namespace SferumNet.Services;

public class ScenarioConfigurator(SferumNetContext ef) : IScenarioConfigurator
{
    public DateTime? DateTimeStarted { get; private set; }
    private CancellationToken _cancellationToken = new();

    public async Task RunAsync()
    {
        var profiles = await GetProfilesAsync();

        if (profiles is null)
            return;
        
        DateTimeStarted = DateTime.Now;

        foreach (var profile in profiles)
        {
            var scenarios = await GetScenariosByProfileAsync(profile.Id);

            if(scenarios is null)
                continue;
            
            foreach (var scenario in scenarios)
            {
                // TODO: START
            }
        }
    }

    public Task StopAsync()
    {
        DateTimeStarted = null;

        _cancellationToken = new CancellationToken();
        
        return Task.CompletedTask;
    }

    public async Task RestartAsync()
    {
        await StopAsync();
        await RunAsync();
    }


    private async Task<ICollection<VkProfile>?> GetProfilesAsync()
    {
        return await ef.VkProfiles
            .ToListAsync(cancellationToken: _cancellationToken);
    }

    private async Task<ICollection<Scenario>?> GetScenariosByProfileAsync(long idProfile)
    {
        return await ef.Scenarios
            .Where(sc => sc.IdProfile == idProfile)
            .Where(sc => sc.IsActive)
            .ToListAsync(cancellationToken: _cancellationToken);
    }
}