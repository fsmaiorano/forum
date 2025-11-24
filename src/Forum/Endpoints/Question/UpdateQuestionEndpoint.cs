using Forum.Application.UseCases.Question.UpdateQuestion;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Endpoints.Question;

public record UpdateQuestionCommand();

public record UpdateQuestionResponse(string QuestionId);

public static class UpdateQuestionEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/question",
                async ([FromBody] UpdateQuestionCommand command, [FromServices] IUpdateQuestionUseCase handler) =>
                {
                    // var result = await handler.UpdateQuestionUseCaseHandler(command);
                    // return result.IsSuccess
                    //     ? Results.Ok(new UpdateQuestionResponse(result.Value.QuestionId))
                    //     : Results.BadRequest(result.Error);
                })
            .WithName("UpdateQuestion")
            .WithTags("Question")
            .Produces<UpdateQuestionResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}