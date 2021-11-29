

using APILoggingLibrary.HarJsonObjectModels;
using System.Collections.Generic;

namespace APILoggingLibrary
{
    public class ConfigValues
    {
        public string apiKey { get; set; }
        public Group group { get; set; }
        public Options options { get; set; }
    }

    public class Options
    {
        public List<string> allowList { get; set; }
        public bool isAllowListEmpty { get; set; }
        public List<string> denyList { get; set; }
        public bool isDenyListEmpty { get; set; }
        public bool development { get; set; }
        public int bufferLength { get; set; }
        public string baseLogUrl { get; set; }
    }
}
