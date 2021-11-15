using System.Collections.Generic;

namespace APILoggingLibrary.HarJsonObjectModels
{
    class PostData
    {
        public string mimeType { get; set; }
        public string text { get; set; }
        public string comment { get; set; }

        public List<Params> @params { get; set; }

        public PostData()
        {

        }
        public PostData(string mimeType, string text, string comment, List<Params> @params)
        {
            this.mimeType = mimeType;
            this.text = text;
            this.comment = comment;
            this.@params = @params;
        }
    }
}
