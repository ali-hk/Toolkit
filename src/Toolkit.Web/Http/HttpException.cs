using System;
using System.Runtime.InteropServices;
using System.Text;
using Toolkit.Web.Http;
using Windows.Web.Http;

namespace Toolkit.Web.Http
{
    public class HttpException : Exception
    {
        private readonly HttpRequestMessage _request;
        private readonly HttpResponseMessage _response;

        public HttpException()
        {
        }

        public HttpException(string message)
            : base(message)
        {
        }

        public HttpException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = innerException.HResult;
        }

        public HttpException(HttpRequestMessage request, COMException innerException, string message = null)
            : this(GetRequestExceptionMessage(request, message), innerException)
        {
            HResult = innerException.HResult;
            _request = request;
            _response = null;
        }

        public HttpException(HttpResponseMessage response, COMException innerException, string message = null)
            : this(GetResponseExceptionMessage(response, message), innerException)
        {
            HResult = innerException.HResult;
            _request = response?.RequestMessage;
            _response = response;
        }

        public HttpRequestMessage Request
        {
            get
            {
                return _response?.RequestMessage;
            }
        }

        public HttpResponseMessage Response
        {
            get
            {
                return _response;
            }
        }

        private static string GetRequestExceptionMessage(HttpRequestMessage request, string message)
        {
            if (request == null)
            {
                return message;
            }
            else
            {
                return $"{message}, {request.ToStringDetailed()}";
            }
        }

        private static string GetResponseExceptionMessage(HttpResponseMessage response, string message)
        {
            if (response == null)
            {
                return message;
            }
            else
            {
                return $"{message}, {response.ToStringDetailed()}";
            }
        }
    }
}