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
    }

    public partial class Action
    {
        [JsonProperty("trigger")]
        public string Trigger { get; set; }

        [JsonProperty("triggerActions")]
        public TriggerAction[] TriggerActions { get; set; }
    }

    public partial class TriggerAction
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("messageId")]
        public string MessageId { get; set; }
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
