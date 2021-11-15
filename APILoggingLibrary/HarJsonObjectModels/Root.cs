namespace APILoggingLibrary.HarJsonObjectModels
{
    class Root
    {
        public string _id { get; set; }
        public bool development { get; set; }
        public string clientIPAddress { get; set; }
        public Group group { get; set; }
        public RequestMain request { get; set; }

        public Root(string uuid, bool development, string clientIPAddress, Group group, RequestMain requestMain)
        {
            _id = uuid;
            this.development = development;
            this.clientIPAddress = clientIPAddress;
            this.group = group;
            this.request = requestMain;
        }
    }
}
