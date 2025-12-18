using BuildingBlocks.Middleware;
using Forum.Application;
using Forum.Endpoints.Answer;
using Forum.Endpoints.Question;
using Forum.Infrastructure;
using Forum.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification API v1");
    options.RoutePrefix = "swagger";
});

await app.InitialiseDatabaseAsync();

app.UseExceptionHandler();

app.UseCors();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

GetQuestionsEndpoint.MapEndpoint(app);
GetQuestionByIdEndpoint.MapEndpoint(app);
GetQuestionsByAuthorIdEndpoint.MapEndpoint(app);
CreateQuestionEndpoint.MapEndpoint(app);
UpdateQuestionEndpoint.MapEndpoint(app);
DeleteQuestionEndpoint.MapEndpoint(app);

GetAnswersEndpoint.MapEndpoint(app);
CreateAnswerEndpoint.MapEndpoint(app);
UpdateAnswerEndpoint.MapEndpoint(app);
DeleteAnswerEndpoint.MapEndpoint(app);

app.Run();