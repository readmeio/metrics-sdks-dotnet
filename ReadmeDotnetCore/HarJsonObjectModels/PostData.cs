using Newtonsoft.Json;
using System.Collections.Generic;

namespace ReadmeDotnetCore.HarJsonObjectModels
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    class PostData
    {
        public string mimeType { get; set; }
        public string text { get; set; }
        public string comment { get; set; }

        public List<Params> @params { get; set; }

    }
}
