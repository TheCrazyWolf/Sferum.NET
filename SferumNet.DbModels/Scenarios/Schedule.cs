using SamGK_Api.Interfaces.Client;
using SferumNet.DbModels.Common;

namespace SferumNet.DbModels.Scenarios;

public class Schedule : Scenario
{
    public SheduleSearchType TypeSchedule { get; set; }
    public string Value { get; set; } = string.Empty;
}