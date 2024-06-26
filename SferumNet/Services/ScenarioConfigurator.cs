using Microsoft.EntityFrameworkCore;
using SferumNet.DbModels.Common;
using SferumNet.DbModels.Vk;

using SferumNet.Services.Common;

namespace SferumNet.Services;

public class ScenarioConfigurator(SferumNetContext ef) : IScenarioConfigurator
{
    public DateTime? DateTimeStarted { get; private set; }
    private CancellationTokenSource _cancelTokenSource = new();
    
    public async Task RunAsync()
    {
        _cancelTokenSource = new();
        
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
                
                //Task.Run(() => new WelcomeScenario()))
            }
        }
    }

    public async Task StopAsync()
    {
        DateTimeStarted = null;

        await _cancelTokenSource.CancelAsync();
    }

    public async Task RestartAsync()
    {
        await StopAsync();
        await RunAsync();
    }


    private async Task<ICollection<VkProfile>?> GetProfilesAsync()
    {
        return await ef.VkProfiles
            .ToListAsync(cancellationToken: _cancelTokenSource.Token);
    }

    private async Task<ICollection<Scenario>?> GetScenariosByProfileAsync(long idProfile)
    {
        return await ef.Scenarios
            .Where(sc => sc.IdProfile == idProfile)
            .Where(sc => sc.IsActive)
            .ToListAsync(cancellationToken: _cancelTokenSource.Token);
    }
}