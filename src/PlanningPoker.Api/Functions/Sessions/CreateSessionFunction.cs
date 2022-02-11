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
using PlanningPoker.Domain.DomainModels;

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
            var session = Session.Create();
            log.LogInformation("Created a new session with code {code}", session.Code);

            var userId = Guid.NewGuid().ToString();

            var clientAccess = await psClient.GetClientAccessUriAsync(userId: userId,
                roles: new[] {$"webpubsub.sendToGroup.{session.Code}", $"webpubsub.joinLeaveGroup.{session.Code}" });

            var responseDto = new SessionDetailsDto
            {
                Code = session.Code,
                ConnectionUrl = clientAccess.ToString(),
                UserId = userId,
                Username = sessionPayload.Name
            };

            return new CreatedResult($"/url/{responseDto.Code}", responseDto);

        }
    }
}
