using System;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Begim.Function {
    public static class HttpTriggerCSharp {
        [FunctionName ("HttpTriggerCSharp")]
        public static async Task<IActionResult> Run (
            [HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log) {

            log.LogInformation ("C# HTTP trigger function processed a request.");
            var blobCount = -1;

            string accountName = req.Query["accountName"];
            if (string.IsNullOrEmpty (accountName)) {
                return (ActionResult) new BadRequestObjectResult ("Please pass a storage account name in the query string");
            }
            string containerName = req.Query["containerName"];
            if (string.IsNullOrEmpty (containerName)) {
                return (ActionResult) new BadRequestObjectResult ("Please pass a storage container name in the query string");
            }
            try {
                Uri accountUri = new Uri($"https://{accountName}.blob.core.windows.net/");
                BlobServiceClient client = new BlobServiceClient(accountUri, new DefaultAzureCredential());
                var containerClient=client.GetBlobContainerClient(containerName);
    
                foreach(var blob in containerClient.GetBlobs()  ){
                    blobCount++;
                }


            } catch {
                return (ActionResult) new BadRequestObjectResult ("Unexpected error querying storage service");
            }

            return (ActionResult) new OkObjectResult ($"{blobCount}");
        }

    }

}