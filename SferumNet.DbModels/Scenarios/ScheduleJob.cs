using ClientSamgk.Enums;
using SferumNet.DbModels.Common;

namespace SferumNet.DbModels.Scenarios;

public class ScheduleJob : Job
{
    public ScheduleSearchType TypeSchedule { get; set; } = ScheduleSearchType.Group;
    public string Value { get; set; } = string.Empty;
    public bool IsAddedNextDay { get; set; }
}