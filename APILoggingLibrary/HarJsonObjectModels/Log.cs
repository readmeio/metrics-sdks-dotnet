using System.Collections.Generic;

namespace APILoggingLibrary.HarJsonObjectModels
{
    class Log
    {
        public Creator creator { get; set; }
        public List<Entries> entries { get; set; }

        public Log(Creator creator, List<Entries> entries)
        {
            this.creator = creator;
            this.entries = entries;
        }

    }
}
