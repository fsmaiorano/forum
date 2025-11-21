using Forum.Application.UseCases.Question.CreateQuestion;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Endpoints.Question;

public record CreateQuestionResponse(string QuestionId);

public static class CreateQuestionEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/question",
                async ([FromBody] CreateQuestionCommand command, [FromServices] ICreateQuestionUseCase handler) =>
                {
                    var result = await handler.CreateQuestionUseCaseHandler(command);
                    return result.IsSuccess
                        ? Results.Ok(new CreateQuestionResponse(result.Value.QuestionId))
                        : Results.BadRequest(result.Error);
                })
            .WithName("CreateQuestion")
            .WithTags("Question")
            .Produces<CreateQuestionResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}