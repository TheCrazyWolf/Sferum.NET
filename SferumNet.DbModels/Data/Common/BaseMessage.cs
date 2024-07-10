using SferumNet.DbModels.Common;

namespace SferumNet.DbModels.Data.Common;

public class BaseMessage : Entity
{
    public string Message { get; set; } = string.Empty;
}