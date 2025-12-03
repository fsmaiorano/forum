using BuildingBlocks.Base;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.UseCases.SendNotification;

namespace Notification.Endpoints.Notification;

public record SendNotificationRequest(string RecipientId, string Title, string Content);

public static class SendNotificationEndpoint
{
    private const string Route = "/notification/send";

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Route,
                async ([FromBody] SendNotificationRequest request,
                    [FromServices] ISendNotificationUseCase usecase) =>
                {
                    var command = new SendNotificationCommand(
                        new UniqueEntityId(request.RecipientId),
                        request.Title,
                        request.Content);

                    var result = await usecase.SendNotificationUseCaseHandler(command);
                    return result.IsSuccess
                        ? Results.NoContent()
                        : Results.BadRequest(result.Error);
                })
            .WithName("SendNotification")
            .WithTags("Notification")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}