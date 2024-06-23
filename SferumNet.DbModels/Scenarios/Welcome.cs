using SferumNet.DbModels.Common;

namespace SferumNet.DbModels.Scenarios;

public class Welcome : Scenario
{
    public int TotalExecuted { get; set; }
    public int MaxToExecute { get; set; }
}