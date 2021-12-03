using Newtonsoft.Json;

namespace ReadmeDotnetCore.HarJsonObjectModels
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    class Params
    {
        public string name { get; set; }
        public string value { get; set; }
        public string fileName { get; set; }
        public string contentType { get; set; }
        public string comment { get; set; }
    }
}
