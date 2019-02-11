using Newtonsoft.Json;

namespace dialog_gen_lib.model
{
    public partial class DialogModel
    {
        [JsonProperty("initMessage")]
        public InitMessage InitMessage { get; set; }

        [JsonProperty("actions")]
        public Action[] Actions { get; set; }

        [JsonProperty("messages")]
        public Message[] Messages { get; set; }

        [JsonProperty("defaultMessage")]
        public DefaultMessage DefaultMessage { get; set; }
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

    public partial class DefaultMessage
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public partial class InitMessage
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("options")]
        public string[] Options { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
