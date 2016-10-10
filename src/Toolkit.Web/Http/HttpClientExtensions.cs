using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;

namespace Toolkit.Web.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> SendRequestExtendedAsync(this HttpClient client, HttpRequestMessage request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            try
            {
                return await client.SendRequestAsync(request, completionOption);
            }
            catch (COMException exc)
            {
                throw new HttpException(request, exc);
            }
        }

        public static HttpResponseMessage EnsureSuccessStatusCodeExtended(this HttpResponseMessage response)
        {
            if (response == null)
            {
                return null;
            }

            try
            {
                return response.EnsureSuccessStatusCode();
            }
            catch (COMException exc)
            {
                throw new HttpException(response, exc);
            }
        }

        public static string ToStringDetailed(this HttpRequestMessage request)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"\tRequest: {request?.ToString()}");
            stringBuilder.AppendLine($"\tRequest Content: {request?.Content?.ToString()}");
            return stringBuilder.ToString();
        }

        public static string ToStringDetailed(this HttpResponseMessage response)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"\tStatus Code: {response?.StatusCode}");
            stringBuilder.AppendLine($"\tRequest: {response?.RequestMessage?.ToString()}");
            stringBuilder.AppendLine($"\tRepsonse: {response?.ToString()}");
            stringBuilder.AppendLine($"\tRequest Content: {response?.RequestMessage?.Content?.ToString()}");
            stringBuilder.AppendLine($"\tResponse Content: {response?.Content?.ToString()}");
            return stringBuilder.ToString();
        }
    }
}
