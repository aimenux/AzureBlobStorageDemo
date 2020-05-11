﻿using System.Threading.Tasks;

namespace Lib
{
    public interface IBlobClient
    {
        Task<TBlob> GetBlobAsync<TBlob>(string name) where TBlob : class, IBlobModel;
        Task UploadBlobAsync<TBlob>(TBlob blob) where TBlob : class, IBlobModel;
    }
}
