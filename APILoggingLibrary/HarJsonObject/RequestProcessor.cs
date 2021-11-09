using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace APILoggingLibrary.HarJsonObject
{
    class RequestProcessor
    {
        private HttpRequest _request = null;
        private string _requestBodyData = null;

        public RequestProcessor(HttpRequest request, string requestBodyData)
        {
            _request = request;
            _requestBodyData = requestBodyData;
        }

        public Request ProcessRequest()
        {
            Request requestObj = new Request();
            requestObj.headers = GetHeaders();
            requestObj.headersSize = GetHeadersSize().ToString();
            requestObj.postData = _requestBodyData;
            requestObj.bodySize = _requestBodyData.Length.ToString();
            requestObj.queryString = GetQueryStrings();
            requestObj.cookies = GetCookies();
            requestObj.method = _request.Method;
            requestObj.url = _request.Scheme + "://" + _request.Host.Host + ":" + _request.Host.Port + "" + _request.Path;
            requestObj.httpVersion = _request.Protocol;
            return requestObj;
        }

        private List<Headers> GetHeaders()
        {
            List<Headers> headers = new List<Headers>();
            if (_request.Headers.Count > 0)
            {
                foreach (var reqHeader in _request.Headers)
                {
                    Headers header = new Headers();
                    header.name = reqHeader.Key;
                    header.value = reqHeader.Value;
                    headers.Add(header);
                }
            }
            return headers;
        }
        private long GetHeadersSize()
        {
            long headersSize = 0;
            if (_request.Headers.Count > 0)
            {
                foreach (var reqHeader in _request.Headers)
                {
                    headersSize += reqHeader.Value.ToString().Length;
                }
            }
            return headersSize;
        }
        private List<QueryString> GetQueryStrings()
        {
            List<QueryString> queryStrings = new List<QueryString>();
            if (_request.QueryString.HasValue)
            {
                Microsoft.AspNetCore.Http.QueryString queryString = _request.QueryString;
                string[] qss = queryString.Value.Replace("?", "").Split("&");

                foreach (string qs in qss)
                {
                    string[] a = qs.Split("=");
                    QueryString qString = new QueryString();
                    qString.name = a[0];
                    qString.value = a[1];
                    queryStrings.Add(qString);
                }
            }
            return queryStrings;
        }
        private List<Cookies> GetCookies()
        {
            List<Cookies> cookies = new List<Cookies>();
            if (_request.Cookies.Count > 0)
            {
                foreach (var reqCookie in _request.Cookies)
                {
                    Cookies cookie = new Cookies();
                    cookie.name = reqCookie.Key;
                    cookie.value = reqCookie.Value;
                    cookies.Add(cookie);
                }
            }
            return cookies;
        }

    }
}
