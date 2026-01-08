using Microsoft.OpenApi.Models;
using Notification.Application;
using Notification.Endpoints.Notification;
using Notification.Infrastructure;
using Notification.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);
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
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Notification API",
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

app.UseCors();

ReadNotificationEndpoint.MapEndpoint(app);
SendNotificationEndpoint.MapEndpoint(app);

app.Run();