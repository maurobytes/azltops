using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;

namespace Costumes.API
{
    public static class UpdateCostume
    {
        [FunctionName("UpdateCostume")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "costumes/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "CostumesDB",
                collectionName: "Costumes",
                ConnectionStringSetting = "CosmosDbConnectionString",
                Id = "{id}",
                PartitionKey = "{id}")] Document costume,
            [CosmosDB(
                databaseName: "CostumesDB",
                collectionName: "Costumes",
                ConnectionStringSetting = "CosmosDbConnectionString")] DocumentClient client,
            ILogger log)
        {
            log.LogInformation("UpdateCostume function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string title = data?.title;
            string description = data?.description;
            int? spookyness = data?.spookyness;

            if (string.IsNullOrEmpty(title))
            {
                return new BadRequestObjectResult("Please pass a title in the request body");
            }

            if (costume == null)
            {
                return new NotFoundObjectResult("Boo! Costume not found.");
            }
            else
            {
                costume.SetPropertyValue("title", title);
                costume.SetPropertyValue("description", description);
                costume.SetPropertyValue("spookyness", spookyness);

                await client.ReplaceDocumentAsync(costume);

                return new OkObjectResult(costume);
            }
        }
    }
}
