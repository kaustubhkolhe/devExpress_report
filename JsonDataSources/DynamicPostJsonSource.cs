using System;
using System.Net;
using System.Net.Http;
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
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);

                    if (!string.IsNullOrEmpty(BearerToken))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);
                    }

                    string postData = RequestObject != null ?
                        JsonConvert.SerializeObject(RequestObject) : string.Empty;

                    var content = new StringContent(postData, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(Uri, content).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();

                    return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to retrieve JSON data from {Uri}: {ex.Message}", ex);
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