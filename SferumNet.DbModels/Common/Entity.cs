using System.ComponentModel.DataAnnotations;

namespace SferumNet.DbModels.Common;

public class Entity
{
    [Key] public int Id { get; set; }
}