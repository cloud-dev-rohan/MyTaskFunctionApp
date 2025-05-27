using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MyTaskFunctionApp;

public class FuncGetTask
{
    private readonly ITaskService _taskService;
    private readonly ILogger<FuncGetTask> _logger;

    public FuncGetTask(ITaskService taskService, ILogger<FuncGetTask> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    [Function("GetTask")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "tasks/{id?}")] HttpRequestData req,
        string? id)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            if (Guid.TryParse(id, out var taskId))
            {
                var task = await _taskService.GetByIdAsync(taskId);
                if (task == null)
                {
                    var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                    await notFound.WriteStringAsync("Task not found.");
                    return notFound;
                }

                var foundResponse = req.CreateResponse(HttpStatusCode.OK);
                await foundResponse.WriteAsJsonAsync(task);
                return foundResponse;
            }
            else
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Invalid GUID format.");
                return badRequest;
            }
        }

        var allTasks = await _taskService.GetAllAsync();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(allTasks);
        return response;
    }
}
