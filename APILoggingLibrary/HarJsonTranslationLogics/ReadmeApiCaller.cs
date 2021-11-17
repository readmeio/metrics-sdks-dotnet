
using RestSharp;
using System;
using System.Text;
using System.Threading.Tasks;

namespace APILoggingLibrary.HarJsonTranslationLogics
{
    class ReadmeApiCaller
    {
        string _harJsonObject = null;
        string _apiKey = null;

        public ReadmeApiCaller(string harJsonObject, string apiKey)
        {
            _harJsonObject = harJsonObject;
            _apiKey = apiKey;
        }

        public async Task<string> SendHarObjToReadmeApi()
        {
            var client = new RestClient("https://metrics.readme.io/request");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
         
            string apiKey = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_apiKey));

            //Basic V3RCbllRYWg4Vzh0TWdmOEVoV1NsQlVTSFN0V3kzTHc6
            request.AddHeader("Authorization", "Basic V3RCbllRYWg4Vzh0TWdmOEVoV1NsQlVTSFN0V3kzTHc6");
            request.AddParameter("application/json", _harJsonObject, ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);
            return response.Content;
        }

    }
}
