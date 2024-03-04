using MyRoboAdvisor.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();

var app = builder.Build();

await app.ConfigurePipeline();

await app.RunAsync();
