using SferumSharp.Models.Responces;
using SferumSharp.Services;

namespace SferumSharp.Scenario.Base;

public interface IScenario
{
    /// <summary>
    /// Вызов события
    /// </summary>
    /// <param name="vkFactory">Экземпляр сервиса API VK</param>
    /// <param name="currentAccountVkMe">Текущий аккаунт</param>
    /// <returns></returns>
    public Task Handle(VkFactory vkFactory, AccountVkMe currentAccountVkMe);
}