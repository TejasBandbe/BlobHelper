using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BlobFunctions
{
    public class BlobUploadOutputBinding
    {
        private readonly ILogger<BlobUploadOutputBinding> _logger;

        public BlobUploadOutputBinding(ILogger<BlobUploadOutputBinding> logger)
        {
            _logger = logger;
        }

        //[Function("BlobUploadOutputBinding")]
        //[BlobOutput("uploadedblobs/{blobName}", Connection = "BlobConnectionString")]
        //public async Task<byte[]> Run(
        //    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, string blobName)
        //{
        //    #region variables
        //    string requestId = Guid.NewGuid().ToString();
        //    byte[] fileBytes;
        //    #endregion
        //    try
        //    {
        //        var file = req.Form.Files["file"];
        //        var fileSize = Math.Round((double)file.Length / (1024 * 1024), 2);
        //        blobName = $"blob-{requestId}-{file.FileName}";
        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(memoryStream);
        //            fileBytes = memoryStream.ToArray();
        //        }

        //        return fileBytes;
        //    }
        //    catch (Exception ex)
        //    {
        //        var responseObj = new
        //        {
        //            Error = ex.Message + " / " + ex.StackTrace
        //        };
        //        //log error
        //        return new byte[0];
        //    }
        //}
    }
}
