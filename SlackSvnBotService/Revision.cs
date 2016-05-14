using Newtonsoft.Json;

namespace SlackSvnBotService
{
    [JsonObject("payload")]
    public class Revision
    {
        [JsonProperty("revision")]
        public string RevisionId { get; set; }
        [JsonProperty("url")]
        public string Server { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("log")]
        public string Log { get; set; }
    }
}
