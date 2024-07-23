using Microsoft.EntityFrameworkCore;
using SferumNet.Database;
using SferumNet.DbModels.Common;
using SferumNet.DbModels.Scenarios;
using SferumNet.DbModels.Vk;
using SferumNet.Scenarios;
using SferumNet.Scenarios.Common;
using SferumNet.Services.Common;
using WelcomeJob = SferumNet.DbModels.Scenarios.WelcomeJob;
// ReSharper disable AsyncVoidLambda

namespace SferumNet.Services;

public class ScenarioConfigurator : IScenarioConfigurator
{
    public DateTime? DateTimeStarted { get; set; }
    
    private readonly IServiceScopeFactory _scopeFactory;

    private List<Thread> _threads = new();
    private List<BaseJob> _jobs = new();

    public ScenarioConfigurator(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        
        _ = RunAsync();
    }

    public async Task RunAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var ef = scope.ServiceProvider.GetRequiredService<SferumNetContext>();

        var profiles = await GetProfilesAsync(ef);

        if (profiles is null)
            return;

        DateTimeStarted = DateTime.Now;

        foreach (var profile in profiles)
        {
            var scenarios = await GetJobsByProfileAsync(ef, profile.Id);

            if (scenarios is null)
                continue;

            foreach (var scenario in scenarios)
            {
                var job = scenario switch
                {
                    FactJob => JobFactory<FactsJob>(scenario.Id),
                    ScheduleJob => JobFactory<SchedulesJob>(scenario.Id),
                    WelcomeJob => JobFactory<WelcomesJob>(scenario.Id),
                    _ => throw new ArgumentOutOfRangeException(nameof(scenario))
                };
                
                _jobs.Add(job);
                var thread = new Thread(() => _ = job.ExecuteAsync());
                _threads.Add(thread);
                thread.Start();
            }
        }
    }

    private BaseJob JobFactory<T>(long scenarioId) where T : BaseJob
    {
        return (T)Activator.CreateInstance(typeof(T), _scopeFactory, scenarioId)!;
    }

    public Task StopAsync()
    {
        DateTimeStarted = null;
        
        foreach (var job in _jobs.ToList())
        {
            job.Stop();
            _jobs.Remove(job);
        }
        
        foreach (var thread in _threads.ToList())
        {
            thread.Interrupt();
            _threads.Remove(thread);
        }
        
        return Task.CompletedTask;
    }

    public async Task RestartAsync()
    {
        await StopAsync();
        await RunAsync();
    }

    private async Task<ICollection<VkProfile>?> GetProfilesAsync(SferumNetContext ef)
    {
        return await ef.VkProfiles
            .ToListAsync();
    }

    private async Task<ICollection<Job>?> GetJobsByProfileAsync(SferumNetContext ef, long idProfile)
    {
        return await ef.Scenarios
            .Where(sc => sc.IdProfile == idProfile)
            .Where(sc => sc.IsActive)
            .ToListAsync();
    }
}