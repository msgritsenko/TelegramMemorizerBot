using Newtonsoft.Json;
using System;

namespace MemorizerBot;

internal record BotCallbackData(string WidgetName, string Action, string Payload)
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

    public static BotCallbackData FromJsonString(string data)
    {
        return JsonConvert.DeserializeObject<BotCallbackData>(data);
    }

    public static BotCallbackData Build<T>(string widgetName, string action, T payload)
    {
        return new BotCallbackData(widgetName, action, JsonConvert.SerializeObject(payload));
    }
}