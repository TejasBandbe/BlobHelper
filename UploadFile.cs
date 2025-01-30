using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using BlobFunctions.Helpers;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BlobFunctions
{
    public class UploadFile
    {
        private readonly ILogger<UploadFile> _logger;
        private readonly TelemetryClient _telemetryClient;

        public UploadFile(ILogger<UploadFile> logger, TelemetryClient telemetryClient)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        [Function("UploadFile")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            #region variables
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(requestBody);
            string referenceId = data.ReferenceId;
            string systemName = data.SystemName.ToString().ToLower();
            string fullFilePath = data.FullFilePath;
            string file = data.File;

            string fileName = data?.FileName;
            string fileSize = data?.FileSize;
            string blobUrl = string.Empty;
            #endregion

            try
            {
                _telemetryClient.TrackTrace("UploadFile", new Dictionary<string, string>
                {
                    { "ReferenceId", referenceId },
                    { "SourceSystem", data.SourceSystem.ToString() },
                    { "DestinationSystem", data.DestinationSystem.ToString() },
                    { "InterfaceName", data.InterfaceName.ToString() },
                    { "ActivityCategory", data.ActivityCategory.ToString() },
                    { "Activity", data.Activity.ToString() },
                    { "Stage", "UploadFunctionV2 start" }
                });

                BlobHelper blobHelper = new BlobHelper();
                blobUrl = await blobHelper.UploadBase64File(systemName, fullFilePath, file);

                _telemetryClient.TrackTrace("UploadFile", new Dictionary<string, string>
                {
                    { "ReferenceId", referenceId },
                    { "SourceSystem", data.SourceSystem.ToString() },
                    { "DestinationSystem", data.DestinationSystem.ToString() },
                    { "InterfaceName", data.InterfaceName.ToString() },
                    { "ActivityCategory", data.ActivityCategory.ToString() },
                    { "Activity", data.Activity.ToString() },
                    { "Stage", "UploadFunctionV2 complete"},
                    { "BlobUrl", blobUrl}
                });

                if (string.IsNullOrEmpty(blobUrl))
                {
                    var responseObj = new
                    {
                        Message = "Something went wrong"
                    };
                    return new BadRequestObjectResult(responseObj);
                }
                else
                {
                    var responseObj = new
                    {
                        Message = "File uploaded successfully",
                        BlobUrl = blobUrl,
                    };
                    return new OkObjectResult(responseObj);
                }
                
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex, new Dictionary<string, string>
                {
                    { "ReferenceId", referenceId },
                    { "SourceSystem", data.SourceSystem.ToString() },
                    { "DestinationSystem", data.DestinationSystem.ToString() },
                    { "InterfaceName", data.InterfaceName.ToString() },
                    { "ActivityCategory", data.ActivityCategory.ToString() },
                    { "Activity", data.Activity.ToString() },
                    { "Stage", "UploadFunctionV2 Error"}
                });
                var responseObj = new
                {
                    ErrorMessage = ex.Message,
                    ErrorStackTrace = ex.StackTrace,
                };
                return new BadRequestObjectResult(responseObj);
            }
        }
    }
}
