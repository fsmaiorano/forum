using BuildingBlocks.Base;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.UseCases.ReadNotification;

namespace Notification.Endpoints.Notification;

public record ReadNotificationRequest(string NotificationId, string RecipientId);

// public record ReadNotificationResponse();

public static class ReadNotificationEndpoint
{
    private const string Route = "/notification/read";

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Route,
                async ([FromBody] ReadNotificationRequest request, [FromServices] IReadNotificationUseCase usecase) =>
                {
                    var command = new ReadNotificationCommand(new UniqueEntityId(request.NotificationId),
                        new UniqueEntityId(request.RecipientId));

                    var result = await usecase.ReadNotificationUseCaseHandler(command);
                    return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
                })
            .WithName("ReadNotification")
            .WithTags("Notification")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        ;
    }
}