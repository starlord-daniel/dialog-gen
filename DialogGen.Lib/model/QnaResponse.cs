using Newtonsoft.Json;

namespace DialogGen.Lib.Model
{
    public partial class QnaResponse
    {
        [JsonProperty("answers")]
        public Answer[] Answers { get; set; }
    }

    public partial class Answer
    {
        [JsonProperty("answer")]
        public string Text { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("questions")]
        public string[] Questions { get; set; }
    }
}


