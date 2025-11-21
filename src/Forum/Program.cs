using Forum.Application;
using Forum.BuildingBlocks.Middleware;
using Forum.Endpoints.Question;
using Forum.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

CreateQuestionEndpoint.MapEndpoint(app);

app.Run();