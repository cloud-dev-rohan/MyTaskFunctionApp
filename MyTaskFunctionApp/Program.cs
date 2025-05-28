using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyTaskFunctionApp.Data;
using MyTaskFunctionApp;
using Microsoft.EntityFrameworkCore;
using MyTaskFunctionApp.Model;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .AddDbContext<TaskDbContext>(opt => opt.UseInMemoryDatabase("TasksDb")) // In Memory Database
    //Dependancy Injection
    .AddScoped<ITaskService, TaskService>();


builder.Services.Configure<FeatureToggleOptions>(
    builder.Configuration.GetSection("FeatureToggles")); //Option patter for environment settings

builder.Build().Run();
