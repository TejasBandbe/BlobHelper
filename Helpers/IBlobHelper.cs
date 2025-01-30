using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobFunctions.Helpers
{
    public interface IBlobHelper
    {
        #region Upload
        Task<string> UploadBase64File(string containerName, string blobPath, string file);
        #endregion

        #region Read
        Task<string> ReadFileBase64(string fullFileUri);
        Task<byte[]> ReadFileByteArray(string fullFileUri);
        Task<string> ReadFileBase64(string containerName, string filePathInContainer);
        Task<byte[]> ReadFileByteArray(string containerName, string filePathInContainer);
        Task<string> ReadFileBase64(string containerName, string foldersInsideContainerAboveDateFolder, string referenceIdToSearch, string? fileName = null);
        Task<byte[]> ReadFileByteArray(string containerName, string foldersInsideContainerAboveDateFolder, string referenceIdToSearch, string? fileName = null);
        #endregion

        #region Delete
        Task<bool> DeleteFile(string fullFileUri);
        Task<bool> DeleteFile(string containerName, string filePathInContainer);
        Task<bool> DeleteFile(string containerName, string foldersInsideContainerAboveDateFolder, string referenceIdToSearch, string? fileName = null);
        #endregion
    }
}
