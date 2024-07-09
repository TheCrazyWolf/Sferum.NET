using System.ComponentModel.DataAnnotations;

namespace SferumNet.Scenarios;

public enum EnumJob
{
    [Display(Name = "Приветствие")]
    Welcome,
    [Display(Name = "Интересные факты")]
    Facts
}