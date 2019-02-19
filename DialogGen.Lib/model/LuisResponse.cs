using Newtonsoft.Json;

namespace DialogGen.Lib.Model
{
    public partial class LuisResponse
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("topScoringIntent")]
        public Intent TopScoringIntent { get; set; }

        [JsonProperty("intents")]
        public Intent[] Intents { get; set; }

        [JsonProperty("entities")]
        public Entity[] Entities { get; set; }
    }

    public partial class Entity
    {
        [JsonProperty("entity")]
        public string EntityEntity { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("startIndex")]
        public long StartIndex { get; set; }

        [JsonProperty("endIndex")]
        public long EndIndex { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }
    }

    public partial class Intent
    {
        [JsonProperty("intent")]
        public string IntentIntent { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }
    }
}
