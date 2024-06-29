using System.ComponentModel.DataAnnotations.Schema;
using SferumNet.DbModels.Vk;

namespace SferumNet.DbModels.Common;

public class Scenario : Entity
{
    [ForeignKey("IdProfile")]
    public VkProfile? VkProfile { get; set; }
    public long? IdProfile { get; set; }

    public string Title { get; set; } = "Undefined";
    
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    public long IdConversation { get; set; }
    
    public DateTime LastExecuted { get; set; }
    
    public int TotalExecuted { get; set; }
    public int MaxToExecute { get; set; }
    public int Delay { get; set; }
    
    public string Type { get; set; }
    public bool IsActive { get; set; }
}