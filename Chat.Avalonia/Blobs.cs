using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Chat.Avalonia
{
    public class Blobs
    {
        public async Task<string> UploadAsync(string filePath)
        {
            //string path = "/home/dzeju/RiderProjects/blob_test/blob_test/xing.bmp";
            
            string connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";
            string containerName = "chat";
            
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
            
            if (!await container.ExistsAsync())
            {
                await container.CreateAsync();
                
            }
           
            string fileName = Guid.NewGuid() + ".bmp";
                
            BlobClient blob = container.GetBlobClient(fileName);

            await using FileStream file = File.OpenRead(filePath);
            
            await blob.UploadAsync(file);
            
            return fileName;
        }
        
        public async Task<string> DownloadImageAsync(string blobName)
        {
            string connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";
            string downloadPath = "Received/" + blobName + ".bmp"; 
            
            BlobClient blobClient = new BlobClient(connectionString, "chat", blobName);
            await blobClient.DownloadToAsync(downloadPath);
            
            return downloadPath;
        }
    }
}