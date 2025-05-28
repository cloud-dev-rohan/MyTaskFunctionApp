using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyTaskFunctionApp.Model;
using System.Net;

namespace MyTaskFunctionApp;

public class FuncDeleteTask
{
    private readonly ITaskService _taskService;
    private readonly ILogger<FuncDeleteTask> _logger;
    private readonly IOptions<FeatureToggleOptions> _featureOptions;

    public FuncDeleteTask(
        ITaskService taskService,
        ILogger<FuncDeleteTask> logger,
        IOptions<FeatureToggleOptions> featureOptions)
    {
        _taskService = taskService;
        _logger = logger;
        _featureOptions = featureOptions;
    }

    [Function("FnDeleteTask")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "tasks/{id}")] HttpRequestData req,
        string id)
    {
        if (!_featureOptions.Value.EnableTaskDeletion)
        {
            var disabledResponse = req.CreateResponse(HttpStatusCode.Forbidden);
            await disabledResponse.WriteStringAsync("Task deletion is disabled.");
            return disabledResponse;
        }

        if (!Guid.TryParse(id, out var taskId))
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid task ID.");
            return badRequest;
        }

        var deleted = await _taskService.DeleteAsync(taskId);

        if (!deleted)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync("Task not found.");
            return notFound;
        }

        var response = req.CreateResponse(HttpStatusCode.NoContent);
        return response;
    }
}
