using System.ComponentModel.DataAnnotations;

namespace SferumNet.DbModels.Common;

public class Entity
{
    [Key] public long Id { get; set; }
}