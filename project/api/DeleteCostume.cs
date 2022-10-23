using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;

namespace Costumes.API
{
    public static class DeleteCostume
    {
        [FunctionName("DeleteCostume")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "costumes/{id:Guid}")] HttpRequest req,
            [CosmosDB(
                databaseName: "CostumesDB",
                collectionName: "Costumes",
                ConnectionStringSetting = "CosmosDbConnectionString",
                Id = "{id}",
                PartitionKey = "{id}")] dynamic costume,
            [CosmosDB(
                databaseName: "CostumesDB",
                collectionName: "Costumes",
                ConnectionStringSetting = "CosmosDbConnectionString")] DocumentClient client,
            ILogger log)
        {
            log.LogInformation("DeleteCostume function processed a request.");

            if (costume == null)
            {
                return new NotFoundObjectResult("Boo! Costume not found.");
            }
            else
            {
                await client.DeleteDocumentAsync(costume._self, new RequestOptions { PartitionKey = new PartitionKey(costume.id) });

                return new OkObjectResult(costume);
            }
        }
    }
}
