using BuildingBlocks.Base;
using Forum.Application.UseCases.Answer.CreateAnswer;
using Forum.Endpoints.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Endpoints.Answer;

public record CreateAnswerRequest(
    string QuestionId,
    string AuthorId,
    string Content,
    List<AttachmentRequest> Attachments = null!);

public record CreateAnswerResponse(string AnswerId);

public static class CreateAnswerEndpoint
{
    private const string Route = "/answer";

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Route,
                async ([FromBody] CreateAnswerRequest request, [FromServices] ICreateAnswerUseCase handler) =>
                {
                    var command = new CreateAnswerCommand(
                        AuthorId: new UniqueEntityId(request.AuthorId),
                        QuestionId: new UniqueEntityId(request.QuestionId),
                        Content: request.Content, Attachments: request.Attachments?.Select(att =>
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
                    var result = await handler.CreateAnswerUseCaseHandler(command);
                    return result.IsSuccess
                        ? Results.Created("", new CreateAnswerResponse(result.Value.AnswerId.ToString()))
                        : Results.BadRequest(result.Error);
                })
            .WithName("CreateAnswer")
            .WithTags("Answer")
            .Produces<CreateAnswerResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}