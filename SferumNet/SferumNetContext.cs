using Microsoft.EntityFrameworkCore;
using SferumNet.DbModels.Data;
using SferumNet.DbModels.Scenarios;
using SferumNet.DbModels.Vk;

namespace SferumNet;

public class SferumNetContext : DbContext
{
    public DbSet<VkProfile> VkProfiles { get; set; }
    public DbSet<WelcomeSentence> WelcomeSentences { get; set; }
    public DbSet<Flood> ScenarioFloods { get; set; }
    public DbSet<Welcome> ScenarioWelcoms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = LocalStorage.db");
        base.OnConfiguring(optionsBuilder);
    }
}