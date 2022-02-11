using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Messaging.WebPubSub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlanningPoker.Domain;
using PlanningPoker.Domain.DataTransferObjects;
using PlanningPoker.Domain.DomainModels;

namespace PlanningPoker.Api.Functions.Sessions
{
    public static class JoinSessionFunction
    {
        [FunctionName("JoinSessionFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = "session/{code}")] HttpRequestMessage req,
            string code,
            ILogger log)
        {
            var sessionPayload = await req.Content.ReadAsAsync<JoinSessionDto>();
            var webPubSubConnectionString = Environment.GetEnvironmentVariable("WebPubSubConnectionString");
            var psClient = new WebPubSubServiceClient(
                webPubSubConnectionString,
                Constants.PokerHub);

            if (await psClient.GroupExistsAsync(code))
            {
                var pubSubUserId = Guid.NewGuid().ToString();
                var clientAccess = await psClient.GetClientAccessUriAsync(
                    userId: pubSubUserId,
                    roles: new[] { $"webpubsub.sendToGroup.{code}", $"webpubsub.joinLeaveGroup.{code}" }
                    );

                var responseDto = new SessionDetailsDto
                {
                    Code = code,
                    ConnectionUrl = clientAccess.ToString(),
                    UserId = pubSubUserId,
                    Username = sessionPayload.Name
                };

                return new CreatedResult($"/url/{responseDto.Code}", responseDto);
            }

            return new BadRequestResult();
        }
    }
}
