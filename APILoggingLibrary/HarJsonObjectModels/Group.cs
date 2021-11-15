namespace APILoggingLibrary.HarJsonObjectModels
{
    class Group
    {
        public string email { get; set; }
        public string label { get; set; }
        public string id { get; set; }

        public Group(string email, string label, string id)
        {
            this.email = email;
            this.label = label;
            this.id = id;
        }
    }
}
