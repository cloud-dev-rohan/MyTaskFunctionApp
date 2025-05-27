using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyTaskFunctionApp.Data;
using MyTaskFunctionApp;
using Microsoft.EntityFrameworkCore;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
     .AddDbContext<TaskDbContext>(opt => opt.UseInMemoryDatabase("TasksDb"))
     .AddScoped<ITaskService,TaskService>();

builder.Build().Run();
