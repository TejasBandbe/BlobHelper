using BlobFunctions.Helpers;
using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
builder.Services
    .AddApplicationInsightsTelemetryWorkerService(new ApplicationInsightsServiceOptions
    {
        InstrumentationKey = "763f7d1d-f22d-4ab5-b986-6a74a177334e"
    })
    .ConfigureFunctionsApplicationInsights();
KeyVaultHelper.FetchKeyvalutSecrets();

builder.Build().Run();
