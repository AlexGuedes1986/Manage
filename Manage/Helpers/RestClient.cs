using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Helpers
{
    public class RestClient
    {
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        private readonly IConfiguration _configuration;
        private readonly string RapidApiKey;
        private readonly string RapidApiHost;

        public RestClient(IConfiguration configuration)
        {
            _configuration = configuration;
            RapidApiKey = _configuration.GetValue<string>("RapidApiKey");
            RapidApiHost = _configuration.GetValue<string>("RapidApiHost");
            EndPoint = _configuration.GetValue<string>("EndPoint"); 
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
            
        }     

        public string MakeRequest(string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;
            request.Headers.Add("x-rapidapi-key", RapidApiKey);
            request.Headers.Add("x-rapidapi-host", RapidApiHost);

            if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
            {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                request.ContentLength = bytes.Length;

                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                return responseValue;
            }
        }

        public enum HttpVerb
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
