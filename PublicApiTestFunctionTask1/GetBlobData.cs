using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PublicApiTestFunctionTask1
{
    public static class GetBlobData
    {
        [FunctionName("GetBlobData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, Binder binder)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string blobContainer = req.Query["blobContainer"];
            string fileName = req.Query["fileName"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            binder = binder ?? data?.binder;

            string content = "";
            var attributes = new Attribute[]
            {
                new BlobAttribute($"{blobContainer}/{fileName}")                        
            };
            using (var reader = await binder.BindAsync<TextReader>(attributes).ConfigureAwait(false))
            {
                content = reader.ReadToEnd();
            }

            return new OkObjectResult(content);
        }

    }
}
