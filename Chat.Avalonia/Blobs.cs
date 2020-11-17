using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Chat.Avalonia
{
    public static class Blobs
    {
        public static async Task<string> UploadAsync(string filePath)
        {
            const string connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";
            const string containerName = "chat";
            string fileName;
            
            try
            {
                var container = new BlobContainerClient(connectionString, containerName);
            
                if (!await container.ExistsAsync())
                {
                    await container.CreateAsync();
                }
           
                fileName = Guid.NewGuid() + ".bmp";
                
                var blob = container.GetBlobClient(fileName);

                await using var file = File.OpenRead(filePath);
            
                await blob.UploadAsync(file);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return fileName;
        }
        
        public static async Task<string> DownloadImageAsync(string blobName)
        {
            const string connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";
            var downloadPath = "Received/" + blobName + ".bmp"; 
            
            var blobClient = new BlobClient(connectionString, "chat", blobName);
            await blobClient.DownloadToAsync(downloadPath);
            
            return downloadPath;
        }
    }
}