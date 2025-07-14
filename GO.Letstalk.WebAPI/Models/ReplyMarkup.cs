namespace GO.Letstalk.WebAPI.Models
{
    using GO.Letstalk.WebAPI.Enums;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class ReplyMarkup
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "button";

        [JsonProperty(PropertyName = "actions")]
        public List<ReplyAction> Actions { get; set; } = [];
    }

    public class ReplyAction
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }
    }
}