using Newtonsoft.Json;

namespace DialogGen.Lib.Model
{
    public partial class DialogModel
    {
        [JsonProperty("initMessage")]
        public Message InitMessage { get; set; }

        [JsonProperty("actions")]
        public Action[] Actions { get; set; }

        [JsonProperty("messages")]
        public Message[] Messages { get; set; }

        [JsonProperty("defaultMessage")]
        public Message DefaultMessage { get; set; }

        [JsonProperty("qnaSettings")]
        public QnaSettings QnaSettings { get; set;}

        [JsonProperty("luisSettings")]
        public LuisSettings LuisSettings { get; set; }
    }

    public partial class QnaSettings
    {
        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("route")]
        public string Route { get; set; }

        [JsonProperty("endpointKey")]
        public string EndpointKey { get; set; }
    }

    public partial class LuisSettings
    {
        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("appId")]
        public string AppId { get; set; }

        [JsonProperty("Ocp-Apim-Subscription-Key")]
        public string SubscriptionKey { get; set; }

        [JsonProperty("threshold")]
        public float Threshold { get; set; }
    }

    public partial class Action
    {
        [JsonProperty("trigger")]
        public string Trigger { get; set; }

        [JsonProperty("triggerActions")]
        public TriggerAction[] TriggerActions { get; set; }

        [JsonProperty("triggerState")]
        public string TriggerState { get; set; }
    }

    public partial class TriggerAction
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("messageId")]
        public string MessageId { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("options")]
        public string[] Options { get; set; }
    }
}
