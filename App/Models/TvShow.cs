using System;
using Lib;

namespace App.Models
{
    [BlobContainer(ContainerName = "Shows")]
    public class TvShow
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
    }
}