using Forum.Application.UseCases.Answer.UpdateAnswer;
using Forum.Endpoints.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Endpoints.Answer;

public record UpdateAnswerRequest(
    string AnswerId,
    string AuthorId,
    string Content,
    List<AttachmentRequest>? Attachments = null);

// public record UpdateAnswerResponse();

public static class UpdateAnswerEndpoint
{
    private const string Route = "/answer";

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Route,
                async ([FromBody] UpdateAnswerRequest request, [FromServices] IUpdateAnswerUseCase handler) =>
                {
                    var command = new UpdateAnswerCommand(
                        new UniqueEntityId(request.AnswerId),
                        new UniqueEntityId(request.AuthorId),
                        request.Content,
                        request.Attachments?.Select(att =>
                        {
                            AttachmentOwnerType.Of(att.OwnerType);
                            var ownerType = AttachmentOwnerType.FromInt(att.OwnerType);

                            return AttachmentEntity.Create(
                                new UniqueEntityId(att.OwnerId),
                                ownerType.Value,
                                att.Title,
                                att.Link
                            );
                        }).ToList());

                    var result = await handler.UpdateAnswerUseCaseHandler(command);
                    return result.IsSuccess
                        ? Results.NoContent()
                        : Results.BadRequest(result.Error);
                })
            .WithName("UpdateAnswer")
            .WithTags("Answer")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}