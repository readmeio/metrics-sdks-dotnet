using APILoggingLibrary.HarJsonObjectModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace APILoggingLibrary.HarJsonTranslationLogics
{
    class HarJsonBuilder
    {
        private readonly RequestDelegate _next;
        private readonly HttpContext _context;
        private readonly string _userName;
        private readonly string _email;
        private readonly DateTime _startDateTime;

        public HarJsonBuilder(RequestDelegate next, HttpContext context, string userName, string email)
        {
            _next = next;
            _context = context;
            _userName = userName;
            _email = email;
            _startDateTime = DateTime.UtcNow;
        }

        public async Task<string> BuildHar()
        {
            Root harObj = new Root();

            harObj._id = Guid.NewGuid().ToString();
            harObj.development = true;
            harObj.clientIPAddress = _context.Connection.RemoteIpAddress.ToString();
            harObj.group = BuildGroup();
            harObj.request = new RequestMain(await BuildLog());


            return JsonConvert.SerializeObject(new List<Root>() { harObj });
        }

        private Group BuildGroup()
        {
            Group group = new Group();
            group.id = _context.Connection.Id;
            group.label = _userName;
            group.email = _email;
            return group;
        }

        private async Task<Log> BuildLog()
        {
            Log log = new Log();
            log.creator = BuildCreator();
            log.entries = await BuildEntries();
            return log;
        }

        private async Task<List<Entries>> BuildEntries()
        {
            List<Entries> entries = new List<Entries>();

            Entries entry = new Entries();
            entry.pageref = _context.Request.Scheme + "://" + _context.Request.Host.Host + "" + _context.Request.Path;

            //DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            entry.startedDateTime = _startDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            entry.cache = null;
            entry.timing = null;
            entry.request = await BuildRequest();
            entry.response = await BuildResponse();
            entry.time = (int)DateTime.UtcNow.Subtract(_startDateTime).TotalMilliseconds;

            entries.Add(entry);
            return entries;
        }

        private async Task<Response> BuildResponse()
        {
            Response response = null;

            var originalBodyStream = _context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                _context.Response.Body = responseBody;

                await _next.Invoke(_context);

                string responseBodyData = await ProcessResponseBody(_context);
                ResponseProcessor responseProcessor = new ResponseProcessor(_context.Response, responseBodyData);
                response = responseProcessor.ProcessResponse();

                await responseBody.CopyToAsync(originalBodyStream);
            }
            return response;
        }

        private async Task<Request> BuildRequest()
        {
            PostData postData = new PostData();
            if (_context.Request.Method == HttpMethods.Post || _context.Request.Method == HttpMethods.Put || _context.Request.Method == HttpMethods.Patch)
            {
                List<Params> @params = await ProcessRequestBody(_context);
                if (@params.Any())
                {
                    postData.mimeType = _context.Request.ContentType;
                    postData.comment = null;
                    postData.text = null;
                    postData.@params = @params;
                }
            }
            

            RequestProcessor requestProcessor = new RequestProcessor(_context.Request, postData);
            Request request = requestProcessor.ProcessRequest();
            return request; 
        }

        private Creator BuildCreator()
        {
            Creator creator = new Creator();
            creator.name = "readmeio";
            creator.version = "5.0.0";
            creator.comment = GetOS();
            return creator;
        }
        private string GetOS()
        {
            string os = null;
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            { 
                os = "windows";
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                os = "mac";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                os = "linux";
            }
            else
            {
                os = "unknown";
            }
            os = os + "/" + Environment.OSVersion.Version;
            return os;
        }

        private async Task<List<Params>> ProcessRequestBody(HttpContext context)
        {
            StreamReader requestBodyReader = new StreamReader(context.Request.Body);
            string requestBodyData = await requestBodyReader.ReadToEndAsync();
            if (requestBodyData == null || requestBodyData == "")
            {
                context.Request.Body.Position = 0;
                return new List<Params>() { new Params() };
            }

            string[] splitKeyValuesList = requestBodyData.Replace("{", "").Replace("}", "").Replace("\r\n", "").Replace("\"", "").Split(",");
            List<Params> @params = new List<Params>();
            foreach (string KeyValues in splitKeyValuesList)
            {
                string[] nameValue = KeyValues.Trim().Split(':');
                string name = nameValue[0];
                string value = nameValue[1];
                Params param = new Params
                {
                    name = name,
                    value = value,
                    fileName = null,
                    contentType = context.Request.ContentType,
                    comment = null
                };
                @params.Add(param);
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


    }


}
