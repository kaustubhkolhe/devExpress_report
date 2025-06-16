using DevExpress.DataAccess.Json;
using System;

namespace PreciseReportsThree.JsonDataSources
{
    public static class DynamicPostJsonDataSourceHelper
    {
        public static JsonDataSource CreatePostJsonDataSource(string baseUrl, string endpoint, object requestObject, string bearerToken)
        {
            // Combine base URL and endpoint
            var fullUrl = baseUrl.TrimEnd('/') + "/" + endpoint.TrimStart('/');

            var jsonDataSource = new JsonDataSource()
            {
                Name = "dynamicPostJsonDataSource",
                JsonSource = new DynamicPostJsonSource()
                {
                    Uri = new Uri(fullUrl),
                    RequestObject = requestObject,
                    BearerToken = bearerToken
                }
            };
            return jsonDataSource;
        }
    }
}