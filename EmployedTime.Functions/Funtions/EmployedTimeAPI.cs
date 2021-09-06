using EmployedTime.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EmployedTime.Functions.Funtions
{
    public static class EmployedTimeAPI
    {

        [FunctionName(nameof(CreateEmployedTime))]
        public static async Task<IActionResult> CreateEmployedTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "EmployedTime")] HttpRequest req,
            [Table("EmployedTime", Connection = "AzureWebJobsStorage")] CloudTable EmployedTimeTable,
            ILogger log)
        {
            log.LogInformation("Recievied a new employed time");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            common.Models.EmployedTime employedTime = JsonConvert.DeserializeObject<common.Models.EmployedTime>(requestBody);

            if (string.IsNullOrEmpty(employedTime?.Fecha.ToString()))
            {
                return new BadRequestObjectResult(new common.Responses.Response
                {
                    IsSuccess = false,
                    Message = "The request must have a Employed Id."
                });
            }

            EmployedTimeEntity employedTimeEntity = new EmployedTimeEntity
            {
                Fecha = DateTime.UtcNow,
                ETag = "*",
                PartitionKey = "EmployedTime",
                RowKey = Guid.NewGuid().ToString(),
                Tipo = employedTime.Tipo
            };

            TableOperation addOperation = TableOperation.Insert(employedTimeEntity);
            await EmployedTimeTable.ExecuteAsync(addOperation);

            string message = "New employed time stored in table.";
            log.LogInformation(message);

            return new OkObjectResult(new common.Responses.Response
            {
                IsSuccess = true,
                Message = message,
                Result = employedTimeEntity
            });
        }


        [FunctionName(nameof(UpdateEmployedTime))]
        public static async Task<IActionResult> UpdateEmployedTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "EmployedTime/{id}")] HttpRequest req,
            [Table("EmployedTime", Connection = "AzureWebJobsStorage")] CloudTable EmployedTimeTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Update for employed time: {id}, received.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            common.Models.EmployedTime employedTime = JsonConvert.DeserializeObject<common.Models.EmployedTime>(requestBody);

            //Validate Employed Time Id

            TableOperation findOperation = TableOperation.Retrieve<EmployedTimeEntity>("EmployedTime", id);
            TableResult findResult = await EmployedTimeTable.ExecuteAsync(findOperation);

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new common.Responses.Response
                {
                    IsSuccess = false,
                    Message = "Employed Id not found."
                });
            }

            //Update Employed time

            EmployedTimeEntity employedTimeEntity = (EmployedTimeEntity)findResult.Result;

            if (!string.IsNullOrEmpty(employedTime.Fecha.ToString()))
            {
                employedTimeEntity.Fecha = employedTime.Fecha;
                employedTimeEntity.Tipo = employedTime.Tipo;

            }

            TableOperation addOperation = TableOperation.Replace(employedTimeEntity);
            await EmployedTimeTable.ExecuteAsync(addOperation);

            string message = $"Employed time: {id}, updated in table.";
            log.LogInformation(message);

            return new OkObjectResult(new common.Responses.Response
            {
                IsSuccess = true,
                Message = message,
                Result = employedTimeEntity
            });
        }

    }
}