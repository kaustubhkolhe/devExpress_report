using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;

using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Antiforgery;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.Security.Resources;
using DevExpress.XtraReports.Web.Extensions;
using PreciseReportsThree.Services;
using PreciseReportsThree.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCorsPolicy", policyBuilder =>
    {
        policyBuilder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowCredentials();
    });
});

// DevExpress controls
builder.Services.AddDevExpressControls();

builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();

// Controllers with Views
builder.Services.AddControllersWithViews();

builder.Services.ConfigureReportingServices(configurator =>
{
    if (builder.Environment.IsDevelopment()) 
    {
        configurator.UseDevelopmentMode();
    }
    configurator.ConfigureReportDesigner(designerConfigurator =>
    {
        designerConfigurator.RegisterDataSourceWizardJsonConnectionStorage<CustomDataSourceWizardJsonDataConnectionStorage>(true);
        designerConfigurator.RegisterObjectDataSourceWizardTypeProvider<ObjectDataSourceWizardCustomTypeProvider>();
    });
    configurator.ConfigureWebDocumentViewer(viewerConfigurator =>
    {
        viewerConfigurator.UseCachedReportSourceBuilder();
        viewerConfigurator.RegisterJsonDataConnectionProviderFactory<CustomJsonDataConnectionProviderFactory>();
        viewerConfigurator.RegisterConnectionProviderFactory<CustomSqlDataConnectionProviderFactory>();
    });
});

// Database context
builder.Services.AddDbContext<ReportDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ReportsDataConnectionString"))
);

var app = builder.Build();

using (var scope = app.Services.CreateScope()) 
{
    var services = scope.ServiceProvider;
    services.GetRequiredService<ReportDbContext>().InitializeDatabase();
}

var contentDirectoryAllowRule = DirectoryAccessRule.Allow(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "Content")).FullName);
AccessSettings.ReportingSpecificResources.TrySetRules(contentDirectoryAllowRule, UrlAccessRule.Allow());

DevExpress.XtraReports.Configuration.Settings.Default.UserDesignerOptions.DataBindingMode = DevExpress.XtraReports.UI.DataBindingMode.Expressions;

app.UseDevExpressControls();

System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

if (app.Environment.IsDevelopment()) 
{
    app.UseDeveloperExceptionPage();
} 
else 
{
    app.UseExceptionHandler("/Home/Error");

    // HSTS (defaults to 30 days), adjust if needed.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowCorsPolicy");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(PreciseReportsThree.Employees.DataSource));

app.Run();
