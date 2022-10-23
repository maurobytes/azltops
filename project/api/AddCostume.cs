using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Costumes.API
{
    public static class AddCostume
    {
        [FunctionName("AddCostume")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "costumes")] HttpRequest req,
            [CosmosDB(
                databaseName: "CostumesDB",
                collectionName: "Costumes",
                ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("Add Costume function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string title = data?.title;
            string description = data?.description;
            int? spookyness = data?.spookyness;

            if (!string.IsNullOrEmpty(title))
            {
                await documentsOut.AddAsync(new
                {
                    id = System.Guid.NewGuid().ToString(),
                    title = title,
                    description = description,
                    spookyness = spookyness
                });
            }

            string responseMessage = string.IsNullOrEmpty(title)
                ? "Got your request but it didn't contain a title."
                : $"New costume: {title}, has been added to the database.";

            return new OkObjectResult(responseMessage);
        }
    }
}
