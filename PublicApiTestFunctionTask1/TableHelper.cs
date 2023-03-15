using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PublicApiTestFunctionTask1
{
    public static class TableHelper<T> where T : class, ITableEntity
    {
        public static async Task InsertOrMergeAsync( string connectionString, T entity, string tableName)
        {
            try
            {
                var account = CloudStorageAccount.Parse(connectionString);
                var client = account.CreateCloudTableClient();

                CloudTable table = client.GetTableReference(tableName);
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                
            }
            catch (StorageException ex)
            {                
                throw ex;
            }


        }
    }
}
