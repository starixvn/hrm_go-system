using Newtonsoft.Json;

namespace GO.Letstalk.WebAPI.Models.Webhook;

public class EventSource
{
    [JsonProperty(PropertyName = "user_id")]
    public string UserId { get; set; }

    [JsonProperty(PropertyName = "group_id")]
    public string GroupId { get; set; }
}