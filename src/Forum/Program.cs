using BuildingBlocks.Middleware;
using Forum.Application;
using Forum.Endpoints.Answer;
using Forum.Endpoints.Question;
using Forum.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder);

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Forum API",
        Version = "v1",
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Forum API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseExceptionHandler();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

CreateQuestionEndpoint.MapEndpoint(app);
UpdateQuestionEndpoint.MapEndpoint(app);
DeleteQuestionEndpoint.MapEndpoint(app);

CreateAnswerEndpoint.MapEndpoint(app);
UpdateAnswerEndpoint.MapEndpoint(app);
DeleteAnswerEndpoint.MapEndpoint(app);

app.Run();