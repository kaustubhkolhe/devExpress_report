using System;
using System.Net;
using System.Text;
using DevExpress.DataAccess.Json;
using Newtonsoft.Json;

namespace PreciseReportsThree.JsonDataSources
{
    public class DynamicPostJsonSource : UriJsonSource
    {
        public object RequestObject { get; set; }
        public string BearerToken { get; set; }
        public string ContentType { get; set; } = "application/json";

        public override string GetJsonString()
        {
            using (var client = new WebClient())
            {
                // Set content type
                client.Headers[HttpRequestHeader.ContentType] = ContentType;

                // Add Bearer token authentication
                if (!string.IsNullOrEmpty(BearerToken))
                {
                    client.Headers.Add("Authorization", $"Bearer {BearerToken}");
                }

                // Serialize the request object to JSON
                string postData = string.Empty;
                if (RequestObject != null)
                {
                    postData = JsonConvert.SerializeObject(RequestObject);
                }

                // Perform POST request
                byte[] data = Encoding.UTF8.GetBytes(postData);
                byte[] response = client.UploadData(Uri, "POST", data);
                return Encoding.UTF8.GetString(response);
            }
        }

        protected override JsonSourceBase Clone()
        {
            var clone = new DynamicPostJsonSource()
            {
                Uri = Uri,
                RootElement = RootElement,
                RequestObject = RequestObject,
                BearerToken = BearerToken,
                ContentType = ContentType
            };
            return clone;
        }
    }
}