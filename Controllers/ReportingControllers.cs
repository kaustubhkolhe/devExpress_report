//using DevExpress.AspNetCore.Reporting.QueryBuilder;
//using DevExpress.AspNetCore.Reporting.QueryBuilder.Native.Services;
//using DevExpress.AspNetCore.Reporting.ReportDesigner;
//using DevExpress.AspNetCore.Reporting.ReportDesigner.Native.Services;
//using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
//using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;
//using DevExpress.XtraReports.Web.ReportDesigner.Services;
//using DevExpress.XtraReports.Web.ReportDesigner;
//using Microsoft.AspNetCore.Mvc;
//using PreciseReportsThree.Services;
//using PreciseReportsThree.Models;
//using PreciseReportsThree.JsonDataSources;

//namespace PreciseReportsThree.Controllers {
//    public class CustomReportDesignerController : ReportDesignerController
//    {
//        public CustomReportDesignerController(IReportDesignerMvcControllerService controllerService) : base(controllerService)
//        {
//        }

//        [HttpPost("[action]")]
//        public IActionResult GetDesignerModel(
//            [FromForm] string reportUrl,
//            [FromServices] IReportDesignerModelBuilder designerModelBuilder,
//            [FromForm] ReportDesignerSettingsBase designerModelSettings)
//        {
//            // ...
//            var designerModel = designerModelBuilder.Report(reportUrl)
//            // ...
//                .BuildModel();
//            designerModel.Assign(designerModelSettings);
//            return DesignerModel(designerModel);
//        }
//    }

//    public class CustomQueryBuilderController : QueryBuilderController {
//        public CustomQueryBuilderController(IQueryBuilderMvcControllerService controllerService) : base(controllerService) {
//        }
//    }

//    public class CustomWebDocumentViewerController : WebDocumentViewerController {
//        public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService) : base(controllerService) {
//        }
//    }
//}


using DevExpress.AspNetCore.Reporting.QueryBuilder;
using DevExpress.AspNetCore.Reporting.QueryBuilder.Native.Services;
using DevExpress.AspNetCore.Reporting.ReportDesigner;
using DevExpress.AspNetCore.Reporting.ReportDesigner.Native.Services;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using DevExpress.XtraReports.Web.ReportDesigner;
using Microsoft.AspNetCore.Mvc;
using PreciseReportsThree.Services;
using PreciseReportsThree.Models;
using PreciseReportsThree.JsonDataSources;
using System;
using Microsoft.Extensions.Configuration;

namespace PreciseReportsThree.Controllers
{
    public class CustomReportDesignerController : ReportDesignerController
    {
        private readonly IConfiguration _configuration;

        public CustomReportDesignerController(IReportDesignerMvcControllerService controllerService, IConfiguration configuration)
            : base(controllerService)
        {
            _configuration = configuration;
        }

        [HttpPost("[action]")]
        public IActionResult GetDesignerModel(
            [FromForm] string reportUrl,
            [FromServices] IReportDesignerModelBuilder designerModelBuilder,
            [FromForm] ReportDesignerSettingsBase designerModelSettings)
        {
            var designerModel = designerModelBuilder.Report(reportUrl).BuildModel();
            designerModel.Assign(designerModelSettings);
            return DesignerModel(designerModel);
        }

        [HttpPost]
        [Route("GeneratePostReport")]
        public IActionResult GeneratePostReport([FromBody] ReportRequest request)
        {
            try
            {
                var reportService = new DynamicReportService(_configuration);
                var report = reportService.GenerateReportWithPostData(request);

                // Here you can return a success message or the actual report
                return Ok(new { success = true, message = "Report generated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("GenerateMusterReport")]
        public IActionResult GenerateMusterReport([FromBody] ReportGenerationRequest request)
        {
            try
            {
                var reportService = new DynamicReportService(_configuration);
                var report = reportService.GenerateMusterReport(request.RequestData, request.BearerToken);

                return Ok(new { success = true, message = "Muster report generated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("GenerateDynamicReport")]
        public IActionResult GenerateDynamicReport([FromBody] ReportGenerationRequest request)
        {
            try
            {
                var reportService = new DynamicReportService(_configuration);
                var report = reportService.GenerateCustomReport(
                    request.Endpoint,
                    request.RequestData,
                    request.BearerToken
                );

                return Ok(new { success = true, message = "Dynamic report generated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }


    public class CustomQueryBuilderController : QueryBuilderController
    {
        public CustomQueryBuilderController(IQueryBuilderMvcControllerService controllerService) : base(controllerService) { }
    }

    public class CustomWebDocumentViewerController : WebDocumentViewerController
    {
        public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService) : base(controllerService) { }
    }
}
