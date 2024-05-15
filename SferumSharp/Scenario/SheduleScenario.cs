using System.Text;
using SamGK_Api;
using SamGK_Api.Interfaces.Client;
using SamGK_Api.Interfaces.Schedule;
using SferumSharp.Models.Request;
using SferumSharp.Models.Responces;
using SferumSharp.Scenario.Base;
using SferumSharp.Services;

namespace SferumSharp.Scenario;

public class SheduleScenario : IScenario
{
    private const int MorningHour = 7;
    private const int EveningHour = 17;
    private readonly long _chatId;
    private readonly string _idValue;
    private SheduleSearchType _type;
    private DateTime _lastSended = DateTime.MinValue;
    private ClientSamgk _samgk = new();

    public SheduleScenario(long chatId, string id, SheduleSearchType type)
    {
        _chatId = chatId;
        _idValue = id;
        _type = type;
    }


    public async Task Handle(VkFactory vkFactory, AccountVkMe currentAccountVkMe)
    {
        if (ShouldSkipSending())
            return;

        var dates = GetScheduleDates();

        foreach (var date in dates)
        {
            var schedule = await _samgk.Sсhedule.GetScheduleAsync(date, _type, _idValue);
            var messageParams = CreateMessageParams(currentAccountVkMe, schedule);
            
            await vkFactory.MessageSend(messageParams);
            await Task.Delay(1000);
        }

        _lastSended = DateTime.Now;
    }

    private bool ShouldSkipSending()
    {
        if (DateTime.Today == _lastSended.Date)
            return true;

        var currentHour = DateTime.Now.Hour;
        return currentHour <= MorningHour || currentHour >= EveningHour;
    }

    private IEnumerable<DateOnly> GetScheduleDates()
    {
        return new List<DateOnly>
        {
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
    }

    private MessageParams CreateMessageParams(AccountVkMe currentAccountVkMe, IEnumerable<IScheduleDate>? schedule)
    {
        return new MessageParams
        {
            ChatId = _chatId,
            Message = ExtractSchedule(schedule),
            Token = currentAccountVkMe.access_token
        };
    }

    private string ExtractSchedule(IEnumerable<IScheduleDate>? schedule)
    {
        if (schedule is null)
            return "Ошибка при получении расписания";

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