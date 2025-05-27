using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MyTaskFunctionApp.Model;
using System.Net;
using System.Text.Json;

namespace MyTaskFunctionApp;

public class FuncCreateTask
{
    private readonly ITaskService _taskService;
    private readonly ILogger<FuncCreateTask> _logger;

    public FuncCreateTask(ITaskService taskService, ILogger<FuncCreateTask> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    [Function("CreateTask")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "tasks")] HttpRequestData req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var task = JsonSerializer.Deserialize<TaskItem>(requestBody);

        if (string.IsNullOrWhiteSpace(task?.Title))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Title is required.");
            return badResponse;
        }

        var created = await _taskService.CreateAsync(task);
        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(created);

        _logger.LogInformation($"Task created: {created.Title}");
        return response;
    }
}