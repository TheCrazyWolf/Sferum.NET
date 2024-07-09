using Microsoft.EntityFrameworkCore;
using SferumNet.DbModels.Common;
using SferumNet.DbModels.Data;
using SferumNet.DbModels.Scenarios;
using SferumNet.DbModels.Services;
using SferumNet.DbModels.Vk;

namespace SferumNet.Database;

public class SferumNetContext : DbContext
{
    public DbSet<VkProfile> VkProfiles { get; set; }
    public DbSet<WelcomeSentence> WelcomeSentences { get; set; }
    public DbSet<FactSentences> FactsSentences { get; set; }
    public DbSet<Job> Scenarios { get; set; }
    public DbSet<FactJob> FloodsJobs { get; set; }
    public DbSet<WelcomeJob> WelcomJobs { get; set; }
    public DbSet<ScheduleJob> SchedulesJobs { get; set; }
    public DbSet<Log> Logs { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = LocalStorage.db");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Log>()
            .HasOne(x=> x.Scenario)
            .WithMany()
            .HasForeignKey(l => l.IdScenario)
            .OnDelete(DeleteBehavior.SetNull);
        
    }
}