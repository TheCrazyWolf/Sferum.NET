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
    private readonly long _chatId;
    private readonly string _idValue;
    private SheduleSearchType _type;
    private DateTime _lastWelcomed = DateTime.MinValue;
    private ClientSamgk _samgk = new ClientSamgk();

    public SheduleScenario(long chatId, string id, SheduleSearchType type)
    {
        _chatId = chatId;
        _idValue = id;
        _type = type;
    }


    public async Task Handle(VkFactory vkFactory, ResponceAccount currentAccount)
    {
        if (DateTime.Today == _lastWelcomed.Date)
            return;

        if (DateTime.Now.Hour <= 9 && DateTime.Now.Hour >= 22)
            return;

        IEnumerable<DateOnly> dates = new List<DateOnly>()
        {
            DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        };


        foreach (var date in dates)
        {
            var messageParams = new MessageParams
            {
                PeerID = _chatId,
                Message = ExtractShedule(
                    await _samgk.Sсhedule.GetScheduleAsync(date, _type, _idValue)),
                Token = currentAccount.access_token
            };

            await vkFactory.MessageSend(messageParams);
            await Task.Delay(1000);
        }

        _lastWelcomed = DateTime.Now;
    }

    private string ExtractShedule(IEnumerable<IScheduleDate>? schedule)
    {
        if (schedule is null)
            return "Ошибка при получение расписания";

        string msg = "";

        foreach (var scheduleDate in schedule)
        {
            msg += $"Расписание на {scheduleDate.Date}\n";

            foreach (var lesson in scheduleDate.Lessons)
            {
                msg += $"# {lesson.Num} - {lesson.Title} ауд. № {lesson.Cab}\n";
            }
        }

        return msg;
    }
}