using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace PublicApiTestFunctionTask1
{
    public static class Function1
    {
        public class ApiResponse : TableEntity
        {
            public int count { get; set; }
            public List<ResponseEntry> entries { get; set; }
            public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
            public string success { get; set; }
        }

        public class ResponseEntry
        {
            public string API { get; set; }
            public string Description { get; set; }
            public string Auth { get; set; }
            public string HTTPS { get; set; }
            public string Cors { get; set; }
            public string Category { get; set; }
        }

        private static readonly string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        private static readonly string Container = Environment.GetEnvironmentVariable("Container");

        [FunctionName("Function1")]
        public static async Task Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer,  ILogger log, Binder binder)
        {
            try
            {
                String blobContent = "";
                HttpClient client = new HttpClient();
                var httpRequestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api.publicapis.org/random?auth=null")
                };
                ApiResponse response1 = new ApiResponse();
                var response = await client.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    response1 = await response.Content.ReadAsAsync<ApiResponse>();
                    response1.TimeStamp = DateTime.UtcNow;
                    response1.PartitionKey = Guid.NewGuid().ToString();
                    response1.RowKey = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
                    response1.success = "true";
                    blobContent = JsonConvert.SerializeObject(response1);
                }
                else
                {
                    response1.success = "false";
                    response1.PartitionKey = Guid.NewGuid().ToString();
                    response1.RowKey = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
                    response1.TimeStamp = DateTime.UtcNow;
                    blobContent = JsonConvert.SerializeObject(response1);
                }

                await TableHelper<ApiResponse>.InsertOrMergeAsync(connectionString, response1, "tableName");

                await BlobHelper.StoreInBlob(binder, Container, blobContent);

                log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
            }
            catch (Exception ex) {
                log.LogInformation($"C# Timer trigger function given error at: {DateTime.UtcNow} " + ex.ToString());
            }                       
        }
    }
}
