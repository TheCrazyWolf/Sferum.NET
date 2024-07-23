using System.Text;
using SamGK_Api;
using SamGK_Api.Interfaces.Schedule;
using SferumNet.DbModels.Scenarios;

namespace SferumNet.Scenarios;

public class SchedulesJob : WelcomesJob
{
    private readonly ClientSamgk _samgk = new();

    public SchedulesJob(IServiceScopeFactory scopeFactory, long idScenario) : base(scopeFactory, idScenario)
    {
    }

    protected override async Task<string> GetSentencesAsync()
    {
        if (CurrentJob is not ScheduleJob upCasted)
            return $"Не удалось получить расписание. {new Random().Next()}";

        if (upCasted.IsAddedNextDay)
            return await BuilerStringShedule(await _samgk.Sсhedule
                .GetScheduleAsync(DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(1)), upCasted.TypeSchedule, upCasted.Value));
        
        return await BuilerStringShedule(await _samgk.Sсhedule
            .GetScheduleAsync(DateOnly.FromDateTime(DateTime.Now), upCasted.TypeSchedule, upCasted.Value));
    }

    private Task<string> BuilerStringShedule(IEnumerable<IScheduleDate>? schedule)
    {
        if (schedule is null || schedule.Count() is 0)
            return Task.FromResult($"Не удалось получить расписание. {new Random().Next()}");

        var msg = new StringBuilder();

        foreach (var scheduleDate in schedule)
        {
            msg.AppendLine($"Расписание на {scheduleDate.Date}");
            foreach (var lesson in scheduleDate.Lessons)
            {
                msg.AppendLine($"# {lesson.Num} - {lesson.Title} ауд. № {lesson.Cab}");
            }
        }

        return Task.FromResult(msg.ToString());
    }
}