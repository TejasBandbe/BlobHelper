using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using BlobFunctions.Helpers;

namespace BlobFunctions
{
    public class DeleteFile
    {
        private readonly ILogger<DeleteFile> _logger;
        private readonly TelemetryClient _telemetryClient;

        public DeleteFile(ILogger<DeleteFile> logger, TelemetryClient telemetryClient)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        [Function("DeleteFile")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            #region variables
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(requestBody);
            string referenceId = data.ReferenceId;
            string fullFileUri = data?.FullFileUri;

            string systemName = data?.SystemName;
            if(!string.IsNullOrEmpty(systemName))
            {
                systemName = systemName.ToLower();
            }
            string filePathInContainer = data?.FilePathInContainer;

            string foldersInsideContainerAboveDateFolder = data?.FoldersInsideContainerAboveDateFolder;
            string referenceIdToSearch = data?.ReferenceIdToSearch;
            string fileName = data?.FileName;
            bool isDeleted = false;
            #endregion

            try
            {
                _telemetryClient.TrackTrace("DeleteFile", new Dictionary<string, string>
                {
                    { "ReferenceId", referenceId },
                    { "SourceSystem", data.SourceSystem.ToString() },
                    { "DestinationSystem", data.DestinationSystem.ToString() },
                    { "InterfaceName", data.InterfaceName.ToString() },
                    { "ActivityCategory", data.ActivityCategory.ToString() },
                    { "Activity", data.Activity.ToString() },
                    { "Stage", "DeleteFile start" }
                });

                if (!string.IsNullOrEmpty(fullFileUri))
                {
                    BlobHelper blobHelper = new BlobHelper();
                    isDeleted = await blobHelper.DeleteFile(fullFileUri);
                }
                else if (!string.IsNullOrEmpty(filePathInContainer))
                {
                    BlobHelper blobHelper = new BlobHelper();
                    isDeleted = await blobHelper.DeleteFile(systemName, filePathInContainer);
                }
                else
                {
                    BlobHelper blobHelper = new BlobHelper();
                    isDeleted = await blobHelper.DeleteFile(systemName, foldersInsideContainerAboveDateFolder, referenceIdToSearch, fileName);
                }

                _telemetryClient.TrackTrace("DeleteFile", new Dictionary<string, string>
                {
                    { "ReferenceId", referenceId },
                    { "SourceSystem", data.SourceSystem.ToString() },
                    { "DestinationSystem", data.DestinationSystem.ToString() },
                    { "InterfaceName", data.InterfaceName.ToString() },
                    { "ActivityCategory", data.ActivityCategory.ToString() },
                    { "Activity", data.Activity.ToString() },
                    { "Stage", "DeleteFile complete" }
                });
                
                if(isDeleted)
                {
                    var responseObj = new
                    {
                        Message = "File Deleted"
                    };
                    return new OkObjectResult(responseObj);
                }
                else
                {
                    var responseObj = new
                    {
                        Message = "File not found. Please check FileName / InterfaceName / ReferenceIdToSearch",
                    };
                    return new BadRequestObjectResult(responseObj);
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
                    { "Stage", "DeleteFile Error" }
                });
                Object responseObj = null;
                if (ex.Message.Contains("404"))
                {
                    responseObj = new
                    {
                        ErrorMessage = "File does not exists in given location"
                    };
                    return new NotFoundObjectResult(responseObj);
                }
                else
                {
                    responseObj = new
                    {
                        ErrorMessage = ex.Message,
                        ErrorStackTrace = ex.StackTrace,
                    };
                    return new BadRequestObjectResult(responseObj);
                }
            }
        }
    }
}
