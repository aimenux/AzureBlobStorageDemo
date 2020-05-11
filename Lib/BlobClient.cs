using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Lib.Configuration;
using Newtonsoft.Json;

namespace Lib
{
    public class BlobClient : IBlobClient
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;

        public BlobClient(ISettings settings)
        {
            _blobServiceClient = new BlobServiceClient(settings.ConnectionString);
            _blobContainerClient = GetOrCreateBlobContainer(settings.ContainerName);
        }

        public Task<TBlob> GetBlobAsync<TBlob>(string name) where TBlob : class, IBlobModel
        {
            throw new System.NotImplementedException();
        }

        public async Task UploadBlobAsync<TBlob>(TBlob blob) where TBlob : class, IBlobModel
        {
            var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
            var exists = await blobClient.ExistsAsync();
            if (exists)
            {
                throw new ArgumentException($"Blob {blob.Name} already exists!");
            }
            using (var stream = SerializeToStream(blob.Content))
            {
                await blobClient.UploadAsync(stream, metadata: blob.Metadata);
            }
        }

        private BlobContainerClient GetOrCreateBlobContainer(string containerName)
        {
            var lowerContainerName = containerName.ToLower();
            var containerClient = _blobServiceClient.GetBlobContainerClient(lowerContainerName);
            if (!containerClient.Exists())
            {
                containerClient = _blobServiceClient.CreateBlobContainer(lowerContainerName);
            }

            return containerClient;
        }

        private static MemoryStream SerializeToStream(object obj)
        {
            var stream = new MemoryStream();
            var json = JsonConvert.SerializeObject(obj);
            var writer = new StreamWriter(stream);
            writer.Write(json);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}