using BlobFunctions.Helpers;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BlobFunctions
{
    public class CheckFile
    {
        private readonly ILogger<CheckFile> _logger;
        private readonly TelemetryClient _telemetryClient;

        public CheckFile(ILogger<CheckFile> logger, TelemetryClient telemetryClient)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        [Function("CheckFile")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            #region variables
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(requestBody);
            string referenceId = data.ReferenceId;
            string fullFileUri = data?.FullFileUri;

            string systemName = data?.SystemName;
            if (!string.IsNullOrEmpty(systemName))
            {
                systemName = systemName.ToLower();
            }
            string filePathInContainer = data?.FilePathInContainer;

            string foldersInsideContainerAboveDateFolder = data?.FoldersInsideContainerAboveDateFolder;
            string referenceIdToSearch = data?.ReferenceIdToSearch;
            string fileName = data?.FileName;
            bool isExist = false;
            #endregion

            try
            {
                _telemetryClient.TrackTrace("CheckFile", new Dictionary<string, string>
                {
                    { "ReferenceId", referenceId },
                    { "SourceSystem", data.SourceSystem.ToString() },
                    { "DestinationSystem", data.DestinationSystem.ToString() },
                    { "InterfaceName", data.InterfaceName.ToString() },
                    { "ActivityCategory", data.ActivityCategory.ToString() },
                    { "Activity", data.Activity.ToString() },
                    { "Stage", "CheckFile start" }
                });

                if (!string.IsNullOrEmpty(fullFileUri))
                {
                    BlobHelper blobHelper = new BlobHelper();
                    isExist = await blobHelper.CheckFile(fullFileUri);
                }
                else if (!string.IsNullOrEmpty(filePathInContainer))
                {
                    BlobHelper blobHelper = new BlobHelper();
                    isExist = await blobHelper.CheckFile(systemName, filePathInContainer);
                }
                else
                {
                    BlobHelper blobHelper = new BlobHelper();
                    isExist = await blobHelper.CheckFile(systemName, foldersInsideContainerAboveDateFolder, referenceIdToSearch, fileName);
                }

                _telemetryClient.TrackTrace("CheckFile", new Dictionary<string, string>
                {
                    { "ReferenceId", referenceId },
                    { "SourceSystem", data.SourceSystem.ToString() },
                    { "DestinationSystem", data.DestinationSystem.ToString() },
                    { "InterfaceName", data.InterfaceName.ToString() },
                    { "ActivityCategory", data.ActivityCategory.ToString() },
                    { "Activity", data.Activity.ToString() },
                    { "Stage", "CheckFile complete" }
                });

                if (isExist)
                {
                    var responseObj = new
                    {
                        Message = "File is present"
                    };
                    return new OkObjectResult(responseObj);
                }
                else
                {
                    var responseObj = new
                    {
                        Message = "File is not present"
                    };
                    return new NotFoundObjectResult(responseObj);
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
                    { "Stage", "CheckFile Error"}
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
