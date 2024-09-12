using System.Text;
using ClientSamgk;
using ClientSamgkOutputResponse.Interfaces.Schedule;
using SferumNet.DbModels.Scenarios;

namespace SferumNet.Scenarios;

public class SchedulesJob : WelcomesJob
{
    private readonly ClientSamgkApi _samgk = new();

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

    private Task<string> BuilerStringShedule(IEnumerable<IResultOutScheduleFromDate>? schedule)
    {
        if (schedule is null || schedule.Count() is 0)
            return Task.FromResult($"Не удалось получить расписание. {new Random().Next()}");
            
        var msg = new StringBuilder();

        foreach (var scheduleDate in schedule)
        {
            msg.AppendLine($"Расписание на {scheduleDate.Date}");
            foreach (var lesson in scheduleDate.Lessons)
            {
                msg.AppendLine($"# {lesson.NumPair}.{lesson.NumLesson} - {lesson.SubjectDetails.SubjectName} ({lesson.Identity.FirstOrDefault()?.Name}) ауд. № {lesson.Cabs.FirstOrDefault()?.Adress}");
            }
        }

        return Task.FromResult(msg.ToString());
    }
    
    private Task<string> BuilerStringShedule(IResultOutScheduleFromDate? schedule)
    {
        if (schedule is null || schedule.Lessons.Count() is 0)
            return Task.FromResult($"Не удалось получить расписание. {new Random().Next()}");
            
        var msg = new StringBuilder();

        msg.AppendLine($"Расписание на {schedule.Date}");
        
        foreach (var lesson in schedule.Lessons)
        {
            msg.AppendLine($"# {lesson.NumPair}.{lesson.NumLesson} - {lesson.SubjectDetails.SubjectName} ({lesson.Identity.FirstOrDefault()?.Name}) ауд. № {lesson.Cabs.FirstOrDefault()?.Adress}");
        }

        return Task.FromResult(msg.ToString());
    }
}