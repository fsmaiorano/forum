using BuildingBlocks.Base;
using Forum.Application.UseCases.Question.UpdateQuestion;
using Forum.Endpoints.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Endpoints.Question;

public record UpdateQuestionRequest(
    string QuestionId,
    string AuthorId,
    string Title,
    string Content,
    string? Slug = null,
    List<AttachmentRequest>? Attachments = null);

// public record UpdateQuestionResponse();

public static class UpdateQuestionEndpoint
{
    private const string Route = "/question";

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Route,
                async ([FromBody] UpdateQuestionRequest request, [FromServices] IUpdateQuestionUseCase handler) =>
                {
                    var command = new UpdateQuestionCommand(
                        new UniqueEntityId(request.QuestionId),
                        new UniqueEntityId(request.AuthorId),
                        request.Title,
                        request.Content,
                        request.Slug,
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

                    var result = await handler.UpdateQuestionUseCaseHandler(command);
                    return result.IsSuccess
                        ? Results.NoContent()
                        : Results.BadRequest(result.Error);
                })
            .WithName("UpdateQuestion")
            .WithTags("Question")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}