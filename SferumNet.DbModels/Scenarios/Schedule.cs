using SamGK_Api.Interfaces.Client;
using SferumNet.DbModels.Common;

namespace SferumNet.DbModels.Scenarios;

public class Schedule : Scenario
{
    public SheduleSearchType Type { get; set; }
    public string Value { get; set; }
}