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
            return await BuilerStringShedule(await _samgk.Schedule
                .GetScheduleAsync(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), upCasted.TypeSchedule, upCasted.Value));
        
        return await BuilerStringShedule(await _samgk.Schedule
            .GetScheduleAsync(DateOnly.FromDateTime(DateTime.Now), upCasted.TypeSchedule, upCasted.Value));
    }
    
    
    private Task<string> BuilerStringShedule(IResultOutScheduleFromDate schedule)
    {
        var msg = new StringBuilder();

        msg.AppendLine($"Расписание на {schedule.Date}");
        
        foreach (var lesson in schedule.Lessons)
        {
            msg.AppendLine($"=====================");
            msg.AppendLine($"{lesson.NumPair}.{lesson.NumLesson}");
            msg.AppendLine($"{GetShortDiscipline(lesson.SubjectDetails.SubjectName)}");
            msg.AppendLine($"{lesson.Identity.First().GetShortName()}");
            msg.AppendLine($"Каб: {lesson.Cabs.FirstOrDefault()?.Adress} • {lesson.EducationGroup.Name}");
        }

        if (schedule.Lessons.Count is not 0) return Task.FromResult(msg.ToString());
        
        msg.AppendLine($"=====================");
        msg.AppendLine($"Пустой ответ");

        return Task.FromResult(msg.ToString());
    }

    private string GetShortDiscipline(string disciplineName)
    {
        var arraysDisciplineName = disciplineName.Split(' ');

        if (arraysDisciplineName.Length <= 3)
            return disciplineName;

        return $"{arraysDisciplineName[0]} {arraysDisciplineName[1]} {arraysDisciplineName[2]}...";
    }
}