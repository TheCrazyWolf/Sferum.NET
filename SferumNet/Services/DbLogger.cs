using SferumNet.DbModels.Enum;
using SferumNet.DbModels.Services;

namespace SferumNet.Services;

public class DbLogger(SferumNetContext ef)
{
    public async Task LogAsync(long idSc, EventType type, string message)
    {
        var log = new Log
        {
            IdScenario = idSc,
            Type = type,
            Message = message,
            DateTime = DateTime.Now,
        };

        await ef.AddAsync(log);
        await ef.SaveChangesAsync();
    }
}