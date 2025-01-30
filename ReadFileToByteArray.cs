using BlobFunctions.Helpers;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BlobFunctions
{
    public class ReadFileToByteArray
    {
        private readonly ILogger<ReadFileToByteArray> _logger;
        private readonly TelemetryClient _telemetryClient;

        public ReadFileToByteArray(ILogger<ReadFileToByteArray> logger, TelemetryClient )
        {
            _logger = logger;
        }

        [Function("ReadFileToByteArray")]
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
            byte[] fileContent = new byte[0];
            #endregion

            try
            {
                _telemetryClient.TrackTrace("ReadFileToByteArray", new Dictionary<string, string>
                {
                    { "ReferenceId", referenceId },
                    { "SourceSystem", data.SourceSystem.ToString() },
                    { "DestinationSystem", data.DestinationSystem.ToString() },
                    { "InterfaceName", data.InterfaceName.ToString() },
                    { "ActivityCategory", data.ActivityCategory.ToString() },
                    { "Activity", data.Activity.ToString() },
                    { "Stage", "ReadFileToByteArray start" }
                });

                if (!string.IsNullOrEmpty(fullFileUri))
                {
                    BlobHelper blobHelper = new BlobHelper();
                    fileContent = await blobHelper.ReadFileByteArray(fullFileUri);
                }
                else if (!string.IsNullOrEmpty(filePathInContainer))
                {
                    BlobHelper blobHelper = new BlobHelper();
                    fileContent = await blobHelper.ReadFileByteArray(systemName, filePathInContainer);
                }
                else
                {
                    BlobHelper blobHelper = new BlobHelper();
                    fileContent = await blobHelper.ReadFileByteArray(systemName, foldersInsideContainerAboveDateFolder, referenceIdToSearch, fileName);
                }

                _telemetryClient.TrackTrace("ReadFileToByteArray", new Dictionary<string, string>
                {
                    { "ReferenceId", referenceId },
                    { "SourceSystem", data.SourceSystem.ToString() },
                    { "DestinationSystem", data.DestinationSystem.ToString() },
                    { "InterfaceName", data.InterfaceName.ToString() },
                    { "ActivityCategory", data.ActivityCategory.ToString() },
                    { "Activity", data.Activity.ToString() },
                    { "Stage", "ReadFileToByteArray complete" }
                });

                if (fileContent.Length == 0)
                {
                    var responseObj = new
                    {
                        Message = "File not found. Please check FileName / InterfaceName / ReferenceIdToSearch",
                        FileContent = fileContent,
                    };
                    return new NotFoundObjectResult(responseObj);
                }
                else
                {
                    var responseObj = new
                    {
                        Message = "File fetched successfully",
                        FileContent = fileContent,
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
                    { "Stage", "ReadFileToByteArray Error"}
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
