using SferumSharp.Models.Responces;
using SferumSharp.Services;

namespace SferumSharp.Scenario.Base;

public interface IScenario
{
    public Task Handle(VkFactory vkFactory, ResponceAccount currentAccount);
}