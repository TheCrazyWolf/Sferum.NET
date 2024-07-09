using System.ComponentModel.DataAnnotations;

namespace SferumNet.Scenarios;

public enum EnumScenario
{
    [Display(Name = "Приветствие")]
    Welcome,
    [Display(Name = "Флуды")]
    Flood
}