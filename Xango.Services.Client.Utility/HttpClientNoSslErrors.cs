using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Xango.Services.Client.Utility
{
    public static class HttpClientNoSslErrors
    {
        public static HttpClient NewClientNoSslErrors(this IHttpClientFactory factory, string name)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            var client = new HttpClient(handler);
            return client;
        }
    }
}
