using APILoggingLibrary.HarJsonTranslationLogics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace APILoggingLibrary
{
    public class RequestResponseLogger
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public RequestResponseLogger(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.Value.Contains("favicon.ico"))
            {
                string apiKey = _configuration.GetSection("readme").GetSection("apiKey").Value;
                if(apiKey.Trim() != null || apiKey.Trim() != "")
                {
                    context.Request.EnableBuffering();
                    HarJsonBuilder harJsonBuilder = new HarJsonBuilder(_next, context, _configuration);

                    string harJsonObj = await harJsonBuilder.BuildHar();
                    Debug.Write(harJsonObj);
                    ReadmeApiCaller readmeApiCaller = new ReadmeApiCaller(harJsonObj, apiKey);
                    //string readmeResponse = await readmeApiCaller.SendHarObjToReadmeApi();
                }
            }
        }



    }

}
