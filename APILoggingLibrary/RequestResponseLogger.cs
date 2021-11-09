using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using APILoggingLibrary.HarJsonObject;

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
            //var jsonObj = "[{\"development\":false,\"group\":{\"email\":\"johnsmithid012@gmail.com\",\"label\":\"John Smith\",\"id\":\"kiy5tLMfG2vTegvTzObjyuVqURHptTNG\"},\"request\":{\"log\":{\"creator\":{\"name\":\"readmeio\",\"version\":\"1.2.1\",\"comment\":\"mac/v10-16.0\"},\"entries\":[{\"request\":{\"headers\":[{\"name\":\"host\",\"value\":\"127.0.0.1:50799\"}],\"queryString\":[{\"name\":\"country\",\"value\":\"usa\"}],\"postData\":{},\"method\":\"GET\",\"url\":\"http://127.0.0.1:50799/test\",\"httpVersion\":\"HTTP/1.1\"},\"response\":{\"headers\":[{\"name\":\"x-powered-by\",\"value\":\"Express\"}],\"content\":{\"text\":\"\\\"\\\\\\\"OK\\\\\\\"\\\"\",\"size\":2,\"encoding\":\"utf-8\"},\"status\":200,\"statusText\":\"OK\"},\"pageref\":\"http://127.0.0.1/test\",\"startedDateTime\":\"2021-10-22T20:54:04.150Z\",\"time\":4}]}},\"_id\":\"c5e3af6b-6ea2-49a8-a32b-10a1caafe9c6\",\"clientIPAddress\":\"127.0.0.1\"}]";
            //var client1 = new RestSharp.RestClient("https://metrics.readme.io/request");
            //var request1 = new RestSharp.RestRequest(RestSharp.Method.POST);
            //request1.AddHeader("Content-Type", "application/json");
            //string apiKeyBase64 = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_apiKey)) + "6";
            ////request1.AddHeader("Authorization", apiKeyBase64);
            //request1.AddHeader("Authorization", "Basic a2l5NXRMTWZHMnZUZWd2VHpPYmp5dVZxVVJIcHRUTkc6");
            //request1.AddParameter("application/json", jsonObj, RestSharp.ParameterType.RequestBody);
            //RestSharp.IRestResponse response1 = client1.Execute(request1);

            context.Request.EnableBuffering();

            string requestBodyData = await ProcessRequestBody(context);
            RequestProcessor requestProcessor = new RequestProcessor(context.Request, requestBodyData);
            Request request = requestProcessor.ProcessRequest();

            Response response = null;

            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next.Invoke(context);

                string responseBodyData = await ProcessResponseBody(context);
                ResponseProcessor responseProcessor = new ResponseProcessor(context.Response, responseBodyData);
                response = responseProcessor.ProcessResponse();

                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> ProcessRequestBody(HttpContext context)
        {
            StreamReader requestBodyReader = new StreamReader(context.Request.Body);
            string requestBodyData = await requestBodyReader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            return requestBodyData;
        }
        private async Task<string> ProcessResponseBody(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            StreamReader responseBodyReader = new StreamReader(context.Response.Body);
            string responseBodyData = await responseBodyReader.ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            return responseBodyData;
        }




        protected void GetIPAddress(HttpRequest request)
        {
            
            //System.Web.HttpContext context = System.Web.HttpContext.Current;
            //string ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            //if (!string.IsNullOrEmpty(ipAddress))
            //{
            //    string[] addresses = ipAddress.Split(',');
            //    if (addresses.Length != 0)
            //    {
            //        return addresses[0];
            //    }
            //}

            //return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        //private string GetGuid()
        //{
        //    var assembly = typeof(Program).Assembly;
        //    var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
        //    var id = attribute.Value;
        //    var applicationId = ((GuidAttribute)typeof(Program).Assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value;
        //    return applicationId;
        //}
    
    
    
    }

}
