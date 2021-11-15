using APILoggingLibrary.HarJsonObjectModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

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

            if (!context.Request.Path.Value.Contains("favicon.ico"))
            {
                DateTime requestTime = DateTime.UtcNow;

                context.Request.EnableBuffering();

                PostData postData = new PostData();
                if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
                {
                    List<Params> @params = await ProcessRequestBody(context);
                    if (@params.Any())
                    {
                        postData.mimeType = context.Request.ContentType;
                        postData.comment = null;
                        postData.text = null;
                        postData.@params = @params;
                    }   
                }

                RequestProcessor requestProcessor = new RequestProcessor(context.Request, postData);
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

                Entries entries = new Entries(
                    pageref: context.Request.Scheme + "://" + context.Request.Host.Host + "" + context.Request.Path,
                    startedDateTime: requestTime.ToString("yyyy-mm-ddThh:mm:ss.s+hh:mm"),
                    time: DateTime.UtcNow.Subtract(requestTime).TotalMilliseconds,
                    cache: null,
                    timing: null,
                    request: request,
                    response: response
                );

                Log log = new Log(new Creator("readmeio", "5.0.0"), new List<Entries> { entries });

                Root root = new Root(
                    Guid.NewGuid().ToString(),
                    true,
                    context.Connection.RemoteIpAddress.ToString(),
                    new Group(_email, _userName, _apiKey),
                    new RequestMain(log)
                    );

                string harJsonObj = JsonConvert.SerializeObject(new List<Root> { root });

            }

        }

        private async Task<List<Params>> ProcessRequestBody(HttpContext context)
        {
            StreamReader requestBodyReader = new StreamReader(context.Request.Body);
            string requestBodyData = await requestBodyReader.ReadToEndAsync();
            if (requestBodyData == null || requestBodyData == "")
            {
                context.Request.Body.Position = 0;
                return null;
            }
                
            string[] splitKeyValuesList = requestBodyData.Replace("{", "").Replace("}", "").Replace("\r\n", "").Replace("\"","").Split(",");
            List<Params> @params = new List<Params>();
            foreach(string KeyValues in splitKeyValuesList)
            {
                string[] nameValue = KeyValues.Trim().Split(':');
                string name = nameValue[0];
                string value = nameValue[1];
                @params.Add(new Params(name, value, null, context.Request.ContentType, null));
            }
            context.Request.Body.Position = 0;
            return @params;
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


    
    }

}
