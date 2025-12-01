using BuildingBlocks.Base;
using Forum.Application.UseCases.Question.CreateQuestion;
using Forum.Endpoints.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Endpoints.Question;

public record CreateQuestionRequest(
    string Title,
    string Content,
    string AuthorId,
    List<AttachmentRequest> Attachments = null!);

public record CreateQuestionResponse(string QuestionId);

public static class CreateQuestionEndpoint
{
    private const string Route = "/question";

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Route,
                async ([FromBody] CreateQuestionRequest request, [FromServices] ICreateQuestionUseCase handler) =>
                {
                    var command = new CreateQuestionCommand(request.Title, request.Content,
                        new UniqueEntityId(request.AuthorId), Attachments: request.Attachments?.Select(att =>
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
                    var result = await handler.CreateQuestionUseCaseHandler(command);
                    return result.IsSuccess
                        ? Results.Created("", new CreateQuestionResponse(result.Value.QuestionId))
                        : Results.BadRequest(result.Error);
                })
            .WithName("CreateQuestion")
            .WithTags("Question")
            .Produces<CreateQuestionResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}