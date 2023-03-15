using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PublicApiTestFunctionTask1
{
    public static class BlobHelper
    {
        public static async Task StoreInBlob(Binder binder, string blobContainer, string content)
        {
            string blobPath = $"{blobContainer}/{DateTime.UtcNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}.txt";
           
            blobPath = $"{blobContainer}/{Guid.NewGuid()}.txt";
            
            var attributes = new Attribute[]
                  {
                      new BlobAttribute(blobPath)
                  };
            using (var writer = await binder.BindAsync<TextWriter>(attributes).ConfigureAwait(false))
            {
                writer.Write(content);
            }
           
        }
    }
}
