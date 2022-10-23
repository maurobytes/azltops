using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Costumes.API
{
    public static class GetCostumeById
    {
        [FunctionName("GetCostumeById")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "costumes/{id:Guid}")] HttpRequest req,
            [CosmosDB(
                databaseName: "CostumesDB",
                collectionName: "Costumes",
                ConnectionStringSetting = "CosmosDbConnectionString",
                Id = "{id}",
                PartitionKey = "{id}")] dynamic costume,
            ILogger log)
        {
            log.LogInformation("GetCostumeById function processed a request.");

            if (costume == null)
            {
                return new NotFoundObjectResult("Boo! Costume not found.");
            }
            else
            {
                return new OkObjectResult(costume);
            }
        }
    }

}
