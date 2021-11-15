namespace APILoggingLibrary.HarJsonObjectModels
{
    class Params
    {
        public string name { get; set; }
        public string value { get; set; }
        public string fileName { get; set; }
        public string contentType { get; set; }
        public string comment { get; set; }

        public Params(string name, string value, string fileName, string contentType, string comment)
        {
            this.name = name;
            this.value = value;
            this.fileName = fileName;
            this.contentType = contentType;
            this.comment = comment;
        }
    }
}
