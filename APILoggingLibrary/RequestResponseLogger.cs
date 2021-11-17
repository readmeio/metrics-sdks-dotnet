using APILoggingLibrary.HarJsonObjectModels;
using APILoggingLibrary.HarJsonTranslationLogics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace APILoggingLibrary
{
    public class RequestResponseLogger
    {
        private readonly RequestDelegate _next;
        private readonly string _apiKey;
        private readonly string _userName;
        private readonly string _email;

        public RequestResponseLogger(RequestDelegate next, string apiKey, string userName, string email)
        {
            _next = next;
            _apiKey = apiKey;
            _userName = userName;
            _email = email;            
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.Value.Contains("favicon.ico"))
            {             
                context.Request.EnableBuffering();
                HarJsonBuilder harJsonBuilder = new HarJsonBuilder(_next, context, _userName, _email);

                string harJsonObj = await harJsonBuilder.BuildHar();
                ReadmeApiCaller readmeApiCaller = new ReadmeApiCaller(harJsonObj, _apiKey);
                //string readmeResponse = await readmeApiCaller.SendHarObjToReadmeApi();
            
            }
        }

        
    
    }

}
