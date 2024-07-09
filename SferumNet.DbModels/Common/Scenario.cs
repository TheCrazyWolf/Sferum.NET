using System.ComponentModel.DataAnnotations.Schema;
using SferumNet.DbModels.Vk;

namespace SferumNet.DbModels.Common;

public class Scenario : Entity
{
    [ForeignKey("IdProfile")]
    public VkProfile? VkProfile { get; set; }
    public long? IdProfile { get; set; }

    public string Title { get; set; } = "Новый сценарий";

    public TimeSpan TimeStart { get; set; } = new (09, 00, 00);
    public TimeSpan TimeEnd { get; set; } = new (19, 00, 00);
    public long IdConversation { get; set; }
    
    public DateTime LastExecuted { get; set; }
    
    public int TotalExecuted { get; set; }
    public int MaxToExecute { get; set; } = 5;
    public int Delay { get; set; } = 5000;

    public bool IsActiveForWeekend { get; set; } = false;
    public bool IsActive { get; set; } = true;
}