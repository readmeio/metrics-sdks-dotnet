namespace APILoggingLibrary.HarJsonObjectModels
{
    class Entries
    {
        public string pageref { get; set; }
        public string startedDateTime { get; set; }
        public double time { get; set; }
        public string cache { get; set; }
        public Timing timing { get; set; }
        public Request request { get; set; }
        public Response response { get; set; }

        public Entries(string pageref, string startedDateTime, double time, string cache, Timing timing, Request request, Response response)
        {
            this.pageref = pageref;
            this.startedDateTime = startedDateTime;
            this.time = time;
            this.cache = cache;
            this.timing = timing;
            this.request = request;
            this.response = response;
        }
    }
}
