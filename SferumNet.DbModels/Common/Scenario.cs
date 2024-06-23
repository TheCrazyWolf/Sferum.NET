using System.ComponentModel.DataAnnotations.Schema;
using SferumNet.DbModels.Vk;

namespace SferumNet.DbModels.Common;

public class Scenario : Entity
{
    [ForeignKey("IdProfile")]
    public VkProfile? VkProfile { get; set; }
    public int? IdProfile { get; set; }
    
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    public long IdConversation { get; set; }
    
    public DateTime? LastExecuted { get; set; }
}