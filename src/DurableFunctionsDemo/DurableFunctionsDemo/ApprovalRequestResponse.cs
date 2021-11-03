using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using DurableFunctionsDemo.Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DurableFunctionsDemo
{
    public class ApprovalRequestResponse
    {
        [FunctionName(nameof(ApprovalRequestResponseHttp))]
        public async Task<IActionResult> ApprovalRequestResponseHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "approval")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient orchestrationClient)
        {
            var response = await req.Content.ReadAsAsync<ApprovalResponse>();

            await orchestrationClient.RaiseEventAsync(response.InstanceId, Constants.Events.Approval, response.Response);

            return new AcceptedResult();
        }
    }
}
