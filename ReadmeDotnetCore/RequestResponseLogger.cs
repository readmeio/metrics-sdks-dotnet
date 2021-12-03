using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ReadmeDotnetCore.HarJsonObjectModels;
using ReadmeDotnetCore.HarJsonTranslationLogics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadmeDotnetCore
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
                ConfigValues configValues = GetConfigValues();
                if(configValues != null)
                {
                    if (configValues.apiKey != null && configValues.apiKey != "")
                    {
                        context.Request.EnableBuffering();
                        HarJsonBuilder harJsonBuilder = new HarJsonBuilder(_next, context, _configuration, configValues);

                        string harJsonObj = await harJsonBuilder.BuildHar();
                        
                        ReadmeApiCaller readmeApiCaller = new ReadmeApiCaller(harJsonObj, configValues.apiKey);
                        string readmeResponse = await readmeApiCaller.SendHarObjToReadmeApi();
                    }
                    else
                    {
                        await _next(context);
                    }
                }  
                else
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }


        private ConfigValues GetConfigValues()
        {
            ConfigValues configValues = new ConfigValues();

            var readme = _configuration.GetSection("readme");
            
            configValues.apiKey = readme.GetSection("apiKey").Value;
            if (configValues.apiKey == null)
            {
                return null;
            }

            configValues.group = new Group
            {
                id = readme.GetSection("group").GetSection("apiKey").Value,
                label = readme.GetSection("group").GetSection("label").Value,
                email = readme.GetSection("group").GetSection("email").Value
            };
            var options = readme.GetSection("options");
            var denyList = options.GetSection("denyList").GetChildren();
            var allowList = options.GetSection("allowList").GetChildren();


            List<string> denyListList = new List<string>();
            foreach (IConfigurationSection section in denyList)
            {
                denyListList.Add(section.Value);
            }
            List<string> allowListList = new List<string>();
            foreach (IConfigurationSection section in allowList)
            {
                allowListList.Add(section.Value);
            }
            Options optionsObj = new Options();
            optionsObj.denyList = denyListList;
            optionsObj.isDenyListEmpty = (denyListList.Count == 0) ? true : false;
            optionsObj.allowList = allowListList;
            optionsObj.isAllowListEmpty = (allowListList.Count == 0) ? true : false;
            if(options.GetSection("development").Value != null)
            {
                optionsObj.development = bool.Parse(options.GetSection("development").Value);
            }
            if (options.GetSection("bufferLength").Value != null)
            {
                optionsObj.bufferLength = int.Parse(options.GetSection("bufferLength").Value);
            }
            if (options.GetSection("baseLogUrl").Value != null)
            {
                optionsObj.baseLogUrl = options.GetSection("baseLogUrl").Value;
            }

            configValues.options = optionsObj;
            return configValues;
        }


    }

}
