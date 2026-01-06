using BuildingBlocks.Base;
using Forum.Application.UseCases.Question.PatchQuestionSetBestAnswer;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Endpoints.Question;

public record PatchQuestionSetBestAnswerRequest(string QuestionId, string AnswerId);

public static class PatchQuestionSetBestAnswerEndpoint
{
    private const string Route = "/question/{questionId}/best-answer";

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(Route,
                async ([FromRoute] string questionId,
                    [FromBody] PatchQuestionSetBestAnswerRequest request,
                    [FromServices] IPatchQuestionSetBestAnswerUseCase handler) =>
                {
                    var command = new PatchQuestionSetBestAnswerCommand(
                        new UniqueEntityId(request.QuestionId),
                        new UniqueEntityId(request.AnswerId));

                    var result = await handler.PatchQuestionSetBestAnswerUseCaseHandler(command);
                    return result.IsSuccess
                        ? Results.NoContent()
                        : Results.BadRequest(result.Error);
                })
            .WithName("SetBestAnswer")
            .WithTags("Question")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}