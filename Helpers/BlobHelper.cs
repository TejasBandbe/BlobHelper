using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlobFunctions.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        #region Upload
        public async Task<string> UploadBase64File(string containerName, string blobPath, string file)
        {
            try
            {
                byte[] fileBytes = Convert.FromBase64String(file);
                BlobContainerClient containerClient = new BlobContainerClient(KeyVaultHelper.BlobConnectionString, containerName);
                containerClient.CreateIfNotExists();
                var blobClient = containerClient.GetBlobClient(blobPath);
                using (var uploadStream = new MemoryStream(fileBytes))
                {
                    await blobClient.UploadAsync(uploadStream, true); //true for overwriting if same file name is already present
                }
                string blobUrl = blobClient.Uri.AbsoluteUri;
                return blobUrl;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region Read
        public async Task<string> ReadFileBase64(string fullFileUri)
        {
            try
            {
                Uri fileUri = new Uri(fullFileUri);
                BlobClient blobClient = new BlobClient(fileUri, new DefaultAzureCredential());
                BlobDownloadInfo download = await blobClient.DownloadAsync();
                using (MemoryStream ms = new MemoryStream())
                {
                    await download.Content.CopyToAsync(ms);
                    byte[] contentBytes = ms.ToArray();
                    return Convert.ToBase64String(contentBytes);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<byte[]> ReadFileByteArray(string fullFileUri)
        {
            try
            {
                Uri fileUri = new Uri(fullFileUri);
                BlobClient blobClient = new BlobClient(fileUri, new DefaultAzureCredential());
                BlobDownloadInfo download = await blobClient.DownloadAsync();
                using (MemoryStream ms = new MemoryStream())
                {
                    await download.Content.CopyToAsync(ms);
                    byte[] contentBytes = ms.ToArray();
                    return contentBytes;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> ReadFileBase64(string containerName, string filePathInContainer)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(KeyVaultHelper.BlobConnectionString, containerName);
                BlobClient blobClient = containerClient.GetBlobClient(filePathInContainer);
                BlobDownloadInfo download = await blobClient.DownloadAsync();
                using (MemoryStream ms = new MemoryStream())
                {
                    await download.Content.CopyToAsync(ms);
                    byte[] contentBytes = ms.ToArray();
                    return Convert.ToBase64String(contentBytes);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<byte[]> ReadFileByteArray(string containerName, string filePathInContainer)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(KeyVaultHelper.BlobConnectionString, containerName);
                BlobClient blobClient = containerClient.GetBlobClient(filePathInContainer);
                BlobDownloadInfo download = await blobClient.DownloadAsync();
                using (MemoryStream ms = new MemoryStream())
                {
                    await download.Content.CopyToAsync(ms);
                    byte[] contentBytes = ms.ToArray();
                    return contentBytes;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> ReadFileBase64(string containerName, string foldersInsideContainerAboveDateFolder, string referenceIdToSearch, string? fileName = null)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(KeyVaultHelper.BlobConnectionString, containerName);

                var blobs = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, prefix: foldersInsideContainerAboveDateFolder);

                var blobItem = blobs.FirstOrDefault(b =>
                    b.Name.Contains($"/{referenceIdToSearch}/") &&
                    (fileName == null || b.Name.EndsWith(fileName))
                );

                if (blobItem != null)
                {
                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                    BlobDownloadInfo download = await blobClient.DownloadAsync();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await download.Content.CopyToAsync(ms);
                        byte[] contentBytes = ms.ToArray();
                        return Convert.ToBase64String(contentBytes);
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<byte[]> ReadFileByteArray(string containerName, string foldersInsideContainerAboveDateFolder, string referenceIdToSearch, string? fileName = null)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(KeyVaultHelper.BlobConnectionString, containerName);

                var blobs = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, prefix: foldersInsideContainerAboveDateFolder);

                var blobItem = blobs.FirstOrDefault(b =>
                    b.Name.Contains($"/{referenceIdToSearch}/") &&
                    (fileName == null || b.Name.EndsWith(fileName))
                );

                if (blobItem != null)
                {
                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                    BlobDownloadInfo download = await blobClient.DownloadAsync();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await download.Content.CopyToAsync(ms);
                        byte[] contentBytes = ms.ToArray();
                        return contentBytes;
                    }
                }
                return new byte[0];
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region Delete
        public async Task<bool> DeleteFile(string fullFileUri)
        {
            try
            {
                Uri fileUri = new Uri(fullFileUri);
                BlobClient blobClient = new BlobClient(fileUri, new DefaultAzureCredential());
                //if soft delete is enabled in storage account blobs, then this will soft delete
                //else permanently delete
                bool isDeleted = await blobClient.DeleteIfExistsAsync(snapshotsOption: DeleteSnapshotsOption.IncludeSnapshots);

                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteFile(string containerName, string filePathInContainer)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(KeyVaultHelper.BlobConnectionString, containerName);
                BlobClient blobClient = containerClient.GetBlobClient(filePathInContainer);
                //if soft delete is enabled in storage account blobs, then this will soft delete
                //else permanently delete
                bool isDeleted = await blobClient.DeleteIfExistsAsync(snapshotsOption: DeleteSnapshotsOption.IncludeSnapshots);
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteFile(string containerName, string foldersInsideContainerAboveDateFolder, string referenceIdToSearch, string? fileName = null)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(KeyVaultHelper.BlobConnectionString, containerName);

                var blobs = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, prefix: foldersInsideContainerAboveDateFolder);

                var blobItem = blobs.FirstOrDefault(b =>
                    b.Name.Contains($"/{referenceIdToSearch}/") &&
                    (fileName == null || b.Name.EndsWith(fileName))
                );

                if (blobItem != null)
                {
                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                    //if soft delete is enabled in storage account blobs, then this will soft delete
                    //else permanently delete
                    bool isDeleted = await blobClient.DeleteIfExistsAsync(snapshotsOption: DeleteSnapshotsOption.IncludeSnapshots);
                   
                    return isDeleted;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region CheckFile
        public async Task<bool> CheckFile(string fullFileUri)
        {
            try
            {
                Uri fileUri = new Uri(fullFileUri);
                BlobClient blobClient = new BlobClient(fileUri, new DefaultAzureCredential());
                bool exists = await blobClient.ExistsAsync();
                return exists;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> CheckFile(string containerName, string filePathInContainer)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(KeyVaultHelper.BlobConnectionString, containerName);
                BlobClient blobClient = containerClient.GetBlobClient(filePathInContainer);
                bool exists = await blobClient.ExistsAsync();
                return exists;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> CheckFile(string containerName, string foldersInsideContainerAboveDateFolder, string referenceIdToSearch, string fileName)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(KeyVaultHelper.BlobConnectionString, containerName);

                var blobs = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, prefix: foldersInsideContainerAboveDateFolder);

                var blobItem = blobs.FirstOrDefault(b =>
                    b.Name.Contains($"/{referenceIdToSearch}/") && b.Name.EndsWith(fileName));

                if (blobItem != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
