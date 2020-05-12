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

        public BlobClient(ISettings settings)
        {
            _blobServiceClient = new BlobServiceClient(settings.ConnectionString);
        }

        public async Task<TBlob> GetBlobAsync<TBlob, TBlobDocument>(string name)
            where TBlob : class, IBlobModel<TBlobDocument>
            where TBlobDocument : class
        {
            var containerName = BlobContainerAttribute.GetContainerName<TBlobDocument>();
            var blobContainerClient = GetOrCreateBlobContainer(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name);
            var exists = await blobClient.ExistsAsync();
            if (!exists) return default;

            var response = await blobClient.DownloadAsync();
            var download = response.Value;
            var reader = new StreamReader(download.Content);
            var jsonContent = await reader.ReadToEndAsync();
            var content = JsonConvert.DeserializeObject<TBlobDocument>(jsonContent);
            var metadata = download.Details.Metadata;

            var blobModel = new BlobModel<TBlobDocument>
            {
                Name = name,
                Content = content,
                Metadata = metadata
            };

            return blobModel as TBlob;
        }

        public async Task SaveBlobAsync<TBlob, TBlobDocument>(TBlob blob)
            where TBlob : class, IBlobModel<TBlobDocument>
            where TBlobDocument : class
        {
            var containerName = BlobContainerAttribute.GetContainerName<TBlobDocument>();
            var blobContainerClient = GetOrCreateBlobContainer(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blob.Name);
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