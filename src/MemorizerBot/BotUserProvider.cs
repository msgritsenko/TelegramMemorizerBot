using Domain;

namespace MemorizerBot;

public class BotUserProvider
{
    private static readonly AsyncLocal<BotUser> _botUserCurrent = new AsyncLocal<BotUser>();

    public BotUser CurrentUser
    {
        get
        {
            return _botUserCurrent.Value;
        }
        set
        {
            _botUserCurrent.Value = value;
        }
    }
}