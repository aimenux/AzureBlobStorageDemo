using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using App.Models;
using Lib;
using Lib.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace App
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "DEV";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            services.Configure<Settings>(configuration.GetSection(nameof(Settings)));
            services.AddSingleton<ISettings>(provider =>
            {
                var options = provider.GetService<IOptions<Settings>>();
                return options.Value;
            });
            services.AddSingleton<IBlobClient, BlobClient>();

            var serviceProvider = services.BuildServiceProvider();
            var blobClient = serviceProvider.GetService<IBlobClient>();

            var name = $"{Guid.NewGuid()}.txt";
            var blobModel = new BlobModel
            {
                Name = name,
                Content = new TvShow
                {
                    Id = "#1",
                    Name = "OnePunchMan",
                    CreationDate = DateTime.UtcNow
                },
                Metadata = new Dictionary<string, string>
                {
                    ["Author"] = "One"
                }
            };

            await blobClient.SaveBlobAsync(blobModel);
            Console.WriteLine($"Blob '{blobModel.Name}' is uploaded");

            var blobFound = await blobClient.GetBlobAsync<BlobModel>(name);
            Console.WriteLine($"Blob content '{blobFound.Content}' is found");

            Console.WriteLine("Press any key to exit !");
            Console.ReadKey();
        }
    }
}
