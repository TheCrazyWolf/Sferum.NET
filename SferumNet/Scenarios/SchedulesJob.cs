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
                .GetScheduleAsync(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), upCasted.TypeSchedule, upCasted.Value));
        
        return await BuilerStringShedule(await _samgk.Sсhedule
            .GetScheduleAsync(DateOnly.FromDateTime(DateTime.Now), upCasted.TypeSchedule, upCasted.Value));
    }
    
    
    private Task<string> BuilerStringShedule(IResultOutScheduleFromDate? schedule)
    {
        if (schedule is null || schedule.Lessons.Count() is 0)
            return Task.FromResult($"Не удалось получить расписание. {new Random().Next()}");
            
        var msg = new StringBuilder();

        msg.AppendLine($"Расписание на {schedule.Date}");
        
        foreach (var lesson in schedule.Lessons
                     .GroupBy(l => new { l.NumPair, l.NumLesson, l.SubjectDetails.SubjectName})
                     .Select(g => g.First())
                     .ToList())
        {
            msg.AppendLine($"=====================");
            msg.AppendLine($"{lesson.NumPair}.{lesson.NumLesson}");
            msg.AppendLine($"{GetShortDiscipline(lesson.SubjectDetails.SubjectName)}");
            msg.AppendLine($"{GetPrepareStringTeacher(lesson.Identity.First().Name)}");
            msg.AppendLine($"Каб: {lesson.Cabs.FirstOrDefault()?.Adress} • {lesson.EducationGroup.Name}");
        }

        return Task.FromResult(msg.ToString());
    }

    private string GetPrepareStringTeacher(string teacherName)
    {
        teacherName = teacherName.Replace("  ", string.Empty)
            .Replace("  ", string.Empty);
        
        var arraysTeacherName = teacherName.Split(' ');

        if (arraysTeacherName.Length == 3)
            return $"{arraysTeacherName[0]} {arraysTeacherName[1].FirstOrDefault()}. {arraysTeacherName[2].FirstOrDefault()}.";

        return teacherName;
    }

    private string GetShortDiscipline(string disciplineName)
    {
        var arraysDisciplineName = disciplineName.Split(' ');

        if (arraysDisciplineName.Length <= 3)
            return disciplineName;

        return $"{arraysDisciplineName[0]} {arraysDisciplineName[1]} {arraysDisciplineName[2]}...";
    }
}