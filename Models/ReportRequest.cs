using System.Collections.Generic;

namespace PreciseReportsThree.Models
{
    public class ReportRequest
    {
        public string BaseUrl { get; set; }
        public string Endpoint { get; set; }
        public object RequestObject { get; set; }  // This can be any object structure
        public string BearerToken { get; set; }
        public string DataMember { get; set; }     // Optional: for nested JSON data
        public string ReportName { get; set; }    // Optional: specify which report to use
    }

    public class ReportGenerationRequest
    {
        public string Endpoint { get; set; }
        public object RequestData { get; set; }
        public string BearerToken { get; set; }
        public string ReportType { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}