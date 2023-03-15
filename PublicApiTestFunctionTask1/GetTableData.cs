using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using Azure;
using System.Linq;

namespace PublicApiTestFunctionTask1
{
    public static class GetTableData
    {
        public class TableData : TableEntity
        {
            public int count { get; set; }
            public string API { get; set; }
            public string Description { get; set; }
            public string Auth { get; set; }
            public string HTTPS { get; set; }
            public string Cors { get; set; }
            public string Category { get; set; }
            public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
            public string success { get; set; }
        }

        private static readonly string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        [FunctionName("GetTableData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            List<TableData> responseList = new List<TableData>();
            string FromDate = req.Query["FromDate"];
            string ToDate =  req.Query["ToDate"];

            string tableName = "DataTable"; // can be changed according to requirement

            TableQuery<TableData> rangeQuery = new TableQuery<TableData>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("TimeStamp", QueryComparisons.GreaterThanOrEqual,
                        FromDate),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("TimeStamp", QueryComparisons.LessThanOrEqual,
                        ToDate)));

            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudTableClient();

            CloudTable cloudTable = client.GetTableReference(tableName);

            var result = cloudTable.ExecuteQuery(rangeQuery);

            responseList = result.ToList();

            return new OkObjectResult(responseList);
           
        }
    }
}
