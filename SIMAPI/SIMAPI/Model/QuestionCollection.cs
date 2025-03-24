using Newtonsoft.Json;

namespace SIMAPI.Model
{

    public class Options
    {
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
    }

    public class QuestionCollection
    {
        [JsonProperty("question")]
        public string QuestionText { get; set; }

        [JsonProperty("options")]
        public Options Options { get; set; }

        [JsonProperty("answer")]
        public string? Answer { get; set; }

        [JsonProperty("explanation")]
        public string? Explanation { get; set; }
    }

    public class Content
    {
        public List<QuestionCollection> Questions { get; set; }
    }
}
