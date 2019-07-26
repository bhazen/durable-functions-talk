using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using DurableFunctionsDemo.Models;

namespace DurableFunctionsDemo
{
    public class ApprovalRequestResponse
    {
        [FunctionName(nameof(ApprovalRequestResponseHttp))]
        public async Task<IActionResult> ApprovalRequestResponseHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "approval")] HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClient orchestrationClient)
        {
            var response = await req.Content.ReadAsAsync<ApprovalResponse>();

            await orchestrationClient.RaiseEventAsync(response.InstanceId, Constants.Events.Approval, response.Response);

            return new AcceptedResult();
        }
    }
}
