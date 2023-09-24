using Newtonsoft.Json;
using System;

namespace MemorizerBot;

internal record BotCallback(string WidgetName, string Action, string Payload)
{
    public T GetPayload<T>()
    {
        if (string.IsNullOrEmpty(Payload))
            return default;

        return JsonConvert.DeserializeObject<T>(Payload);
    }

    public string ToJsonString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static BotCallback FromJsonString(string data)
    {
        return JsonConvert.DeserializeObject<BotCallback>(data);
    }

    public static BotCallback Build<T>(string widgetName, string action, T payload)
    {
        return new BotCallback(widgetName, action, JsonConvert.SerializeObject(payload));
    }
}