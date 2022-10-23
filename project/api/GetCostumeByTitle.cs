using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Costumes.API
{
    public static class GetCostumeByTitle
    {
        [FunctionName("GetCostumeByTitle")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "costumes/{title:alpha}")] HttpRequest req,
            [CosmosDB(
                databaseName: "CostumesDB",
                collectionName: "Costumes",
                ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c WHERE RegexMatch(c.title, {title}, 'i')")]
                 IEnumerable<dynamic> costumes,
            ILogger log)
        {
            log.LogInformation("GetCostumeByTitle function processed a request.");

            if (costumes == null) return new NotFoundResult();

            return new OkObjectResult(costumes);
        }


    }
}
