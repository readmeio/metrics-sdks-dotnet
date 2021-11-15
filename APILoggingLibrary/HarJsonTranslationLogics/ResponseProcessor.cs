using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace APILoggingLibrary.HarJsonObjectModels
{
    class ResponseProcessor
    {
        private HttpResponse _response = null;
        private string _responseBodyData = null;

        public ResponseProcessor(HttpResponse response, string responseBodyData)
        {
            _response = response;
            _responseBodyData = responseBodyData;
        }

        public Response ProcessResponse()
        {
            Response responseObj = new Response();
            responseObj.headers = GetHeaders();
            responseObj.headersSize = GetHeadersSize();
            responseObj.status = _response.StatusCode.ToString(); ;
            responseObj.statusText = GetStatusTextByStatusCode(_response.StatusCode);
            responseObj.content = GetContent();
            responseObj.bodySize = _responseBodyData.Length;
            
            return responseObj;

           
            //--httpVersion
            //--cookies
            //--redirectURL
      
        }

        private List<Headers> GetHeaders()
        {
            List<Headers> headers = new List<Headers>();
            if (_response.Headers.Count > 0)
            {
                foreach (var reqHeader in _response.Headers)
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
            if (_response.Headers.Count > 0)
            {
                foreach (var reqHeader in _response.Headers)
                {
                    headersSize += reqHeader.Value.ToString().Length;
                }
            }
            return headersSize;
        }

        private Content GetContent()
        {
            Content content = new Content();
            content.text = _responseBodyData;
            content.size = _responseBodyData.Length;
            content.mimeType = _response.ContentType;
            return content;
        }



        private List<Cookies> GetCookies()
        {
            List<Cookies> cookies = new List<Cookies>();
            //if (_response.Cookies)
            //{
            //    foreach (var reqCookie in _response.Cookies)
            //    {
            //        Cookies cookie = new Cookies();
            //        cookie.name = reqCookie.Key;
            //        cookie.value = reqCookie.Value;
            //        cookies.Add(cookie);
            //    }
            //}
            return cookies;
        }

        private string GetStatusTextByStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 100: return "Continue";
                case 101: return "Switching Protocols";
                case 102: return "Processing";
                case 200: return "OK";
                case 201: return "Created";
                case 202: return "Accepted";
                case 203: return "Non-Authoritative Information";
                case 204: return "No Content";
                case 205: return "Reset Content";
                case 206: return "Partial Content";
                case 207: return "Multi-Status";
                case 300: return "Multiple Choices";
                case 301: return "Moved Permanently";
                case 302: return "Found";
                case 303: return "See Other";
                case 304: return "Not Modified";
                case 305: return "Use Proxy";
                case 307: return "Temporary Redirect";
                case 400: return "Bad Request";
                case 401: return "Unauthorized";
                case 402: return "Payment Required";
                case 403: return "Forbidden";
                case 404: return "Not Found";
                case 405: return "Method Not Allowed";
                case 406: return "Not Acceptable";
                case 407: return "Proxy Authentication Required";
                case 408: return "Request Timeout";
                case 409: return "Conflict";
                case 410: return "Gone";
                case 411: return "Length Required";
                case 412: return "Precondition Failed";
                case 413: return "Request Entity Too Large";
                case 414: return "Request-Uri Too Long";
                case 415: return "Unsupported Media Type";
                case 416: return "Requested Range Not Satisfiable";
                case 417: return "Expectation Failed";
                case 422: return "Unprocessable Entity";
                case 423: return "Locked";
                case 424: return "Failed Dependency";
                case 500: return "Internal Server Error";
                case 501: return "Not Implemented";
                case 502: return "Bad Gateway";
                case 503: return "Service Unavailable";
                case 504: return "Gateway Timeout";
                case 505: return "Http Version Not Supported";
                case 507: return "Insufficient Storage";
            }
            return "";
        }
 


    }
}
