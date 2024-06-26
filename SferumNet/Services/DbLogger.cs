using SferumNet.DbModels.Enum;
using SferumNet.DbModels.Services;

namespace SferumNet.Services;

public class DbLogger(SferumNetContext ef)
{
    private SferumNetContext _ef = ef;

    public async Task LogAsync(long idSc, EventType type, string message)
    {
        var log = new Log()
        {
            IdScenario = idSc,
            Type = type,
            Message = message,
            DateTime = DateTime.Now,
        };

        await _ef.AddAsync(log);
        await _ef.SaveChangesAsync();
    }
}