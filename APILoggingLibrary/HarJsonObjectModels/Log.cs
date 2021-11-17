using System.Collections.Generic;

namespace APILoggingLibrary.HarJsonObjectModels
{
    class Log
    {
        public Creator creator { get; set; }
        public List<Entries> entries { get; set; }

    }
}
