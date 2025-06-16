using DevExpress.XtraReports.UI;
using PreciseReportsThree.JsonDataSources;
using PreciseReportsThree.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace PreciseReportsThree.Services
{
    public class DynamicReportService
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseApiUrl;

        public DynamicReportService(IConfiguration configuration)
        {
            _configuration = configuration;
            _baseApiUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://your-api.com";
        }

        public XtraReport GenerateReportWithPostData(ReportRequest request)
        {
            try
            {
                // Create the JsonDataSource with the dynamic request object
                var dataSource = DynamicPostJsonDataSourceHelper.CreatePostJsonDataSource(
                    request.BaseUrl ?? _baseApiUrl,
                    request.Endpoint,
                    request.RequestObject,
                    request.BearerToken
                );

                // Create your report (you can load from existing reports or create new)
                var report = new XtraReport();
                report.DataSource = dataSource;

                // If you need to specify a data member for nested JSON
                if (!string.IsNullOrEmpty(request.DataMember))
                {
                    report.DataMember = request.DataMember;
                }

                return report;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}", ex);
            }
        }

        public XtraReport GenerateMusterReport(object requestObject, string bearerToken)
        {
            var dataSource = DynamicPostJsonDataSourceHelper.CreatePostJsonDataSource(
                _baseApiUrl,
                "/reports/muster/mustersummary",
                requestObject,
                bearerToken
            );

            var report = new XtraReport();
            report.DataSource = dataSource;

            // You can configure your specific report layout here
            // or load from existing report templates

            return report;
        }

        public XtraReport GenerateCustomReport(string endpoint, object requestData, string bearerToken)
        {
            var dataSource = DynamicPostJsonDataSourceHelper.CreatePostJsonDataSource(
                _baseApiUrl,
                endpoint,
                requestData,
                bearerToken
            );

            var report = new XtraReport();
            report.DataSource = dataSource;

            return report;
        }
    }
}