using Newtonsoft.Json;

namespace GO.Letstalk.WebAPI.Models.Webhook;

public class PostBackEvent
{
    [JsonProperty("msg")]
    public string Msg { get; set; }

    [JsonProperty("postback")]
    public PostBackData PostBack { get; set; }

    [JsonProperty("is_parsed")]
    public bool? IsParsed { get; set; }

    [JsonProperty("has_url")]
    public bool? HasUrl { get; set; }
}

public class PostBackData
{
    [JsonProperty("data")]
    public string Data { get; set; }
}