using System;

namespace APILoggingLibrary.HarJsonObjectModels
{
    class Creator
    {
        public string name { get; set; }
        public string version { get; set; }
        //comment is OS and its version
        public string comment { get; set; }

        public Creator(string name, string version)
        {
            this.name = name;
            this.version = version;
            comment = Environment.OSVersion.ToString();
        }
    }
}
