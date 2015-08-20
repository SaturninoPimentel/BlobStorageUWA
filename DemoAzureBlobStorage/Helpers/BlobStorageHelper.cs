using System;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DemoAzureBlobStorage.Helpers
{
    public class BlobStorageHelper
    {

        private readonly CloudStorageAccount _storageAccount;

        private string _accountName = "saturblobtest";
        string _key = "cVIkkZ51Je/qRRtlJzk6GBTRv058pdh9z78h9+JRN+6aRZNIAXgbj/jOuOn7tBFV0kY/oIVTOgvKfZrRg/lO3w==";

        public BlobStorageHelper()
        {
            StorageCredentials storageCredentials = new StorageCredentials(_accountName, _key);
            _storageAccount = new CloudStorageAccount(storageCredentials, true);
        }

        public async Task UploadImageAsync(string containerName, string fileName, StorageFile file)
        {
            CloudBlobClient cloudBlobClient = _storageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

            await cloudBlobContainer.CreateIfNotExistsAsync();

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);


            await cloudBlockBlob.UploadFromFileAsync(file);
        }

    }
}
