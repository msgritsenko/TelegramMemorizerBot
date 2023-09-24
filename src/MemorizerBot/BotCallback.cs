using Newtonsoft.Json;

namespace MemorizerBot;

internal record BotCallback(string WidgetName, string Action, string Payload)
{
    public T GetPayload<T>() where T : class
    {
        if (string.IsNullOrEmpty(Payload))
            return null;

        return JsonConvert.DeserializeObject<T>(Payload);
    }
}