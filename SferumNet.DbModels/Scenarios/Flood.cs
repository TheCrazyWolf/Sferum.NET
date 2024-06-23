using SferumNet.DbModels.Common;

namespace SferumNet.DbModels.Scenarios;

public class Flood : Scenario
{
    public int TotalExecuted { get; set; }
    public int MaxToExecute { get; set; }
    public int Delay { get; set; }
}