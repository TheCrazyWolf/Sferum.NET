using System.Text;
using SamGK_Api;
using SamGK_Api.Interfaces.Schedule;
using SferumNet.Database;
using SferumNet.DbModels.Scenarios;
using SferumNet.Services;
using VkNet.Model;

namespace SferumNet.Scenarios;

public class SchedulesJob : WelcomeJob
{
    private readonly ClientSamgk _samgk = new();

    public SchedulesJob(SferumNetContext ef, DbLogger dbLogger, long idScenario) : base(ef, dbLogger, idScenario)
    {
    }

    protected override async Task<string> GetSentencesAsync()
    {
        var upCasted = CurrentJob as ScheduleJob;

        if (upCasted.IsAddedNextDay)
            return await BuilerStringShedule(await _samgk.Sсhedule
                .GetScheduleAsync(DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(1)), upCasted.TypeSchedule, upCasted.Value));
        
        return await BuilerStringShedule(await _samgk.Sсhedule
            .GetScheduleAsync(DateOnly.FromDateTime(DateTime.Now), upCasted.TypeSchedule, upCasted.Value));
    }

    private async Task<string> BuilerStringShedule(IEnumerable<IScheduleDate>? schedule)
    {
        if (schedule is null || schedule.Count() is 0)
            return $"Не удалось получить расписание. {new Random().Next()}";

        var msg = new StringBuilder();

        foreach (var scheduleDate in schedule)
        {
            msg.AppendLine($"Расписание на {scheduleDate.Date}");
            foreach (var lesson in scheduleDate.Lessons)
            {
                msg.AppendLine($"# {lesson.Num} - {lesson.Title} ауд. № {lesson.Cab}");
            }
        }

        return msg.ToString();
    }
}