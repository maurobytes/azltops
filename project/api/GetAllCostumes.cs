using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Costumes.API
{
    public static class GetAllCostumes
    {
        [FunctionName("GetAllCostumes")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "costumes")] HttpRequest req,
            [CosmosDB(
                databaseName: "CostumesDB",
                collectionName: "Costumes",
                ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c")] IEnumerable<dynamic> costumes,
            ILogger log)
        {
            log.LogInformation("GetCostumeById function processed a request.");
            System.Threading.Thread.Sleep(3000);
            return new OkObjectResult(costumes);
        }
    }
}
