using SferumNet.DbModels.Common;
using SferumNet.DbModels.Data.Common;

namespace SferumNet.DbModels.Data;

public class WelcomeSentence : BaseMessage
{
    public string Message { get; set; } = string.Empty;
}