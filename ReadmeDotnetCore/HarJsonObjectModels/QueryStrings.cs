using Newtonsoft.Json;

namespace ReadmeDotnetCore.HarJsonObjectModels
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    class QueryStrings
    {
        public string name { get; set; }  
        public string value { get; set; }  
    }
}
