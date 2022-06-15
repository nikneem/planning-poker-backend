using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Messaging.WebPubSub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PlanningPoker.Domain;
using PlanningPoker.Domain.DataTransferObjects;

namespace PlanningPoker.Api.Functions.Sessions
{
    public static class CreateSessionFunction
    {
        [FunctionName("CreateSessionFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "session")] HttpRequestMessage req,
            ILogger log)
        {
            var webPubSubConnectionString = Environment.GetEnvironmentVariable("WebPubSubConnectionString");
            var psClient = new WebPubSubServiceClient(
                webPubSubConnectionString,
                Constants.PokerHub);

            var sessionPayload = await req.Content.ReadAsAsync<CreateSessionDto>();
            var created = string.IsNullOrWhiteSpace(sessionPayload.Code);
            var sessionCode = sessionPayload.Code ?? Randomizer.GenerateSessionCode();

            log.LogInformation("Created a new session with code {code}", sessionCode);

            var userId = Guid.NewGuid().ToString();

            var clientAccess = await psClient.GetClientAccessUriAsync(
                userId: userId,
                roles: new[]
                {
                    $"webpubsub.sendToGroup.{sessionCode}", 
                    $"webpubsub.joinLeaveGroup.{sessionCode}"
                });

            var responseDto = new SessionDetailsDto
            {
                Code = sessionCode,
                ConnectionUrl = clientAccess.ToString(),
                UserId = userId,
                Username = sessionPayload.Name,
                OwningCode = created ? sessionCode : string.Empty
            };

            return new CreatedResult($"/url/{responseDto.Code}", responseDto);

        }
    }
}
