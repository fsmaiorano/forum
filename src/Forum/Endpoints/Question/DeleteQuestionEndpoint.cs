using Forum.Application.UseCases.Question.DeleteQuestion;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Endpoints.Question;

public record DeleteQuestionRequest();

// public record DeleteQuestionResponse();

public class DeleteQuestionEndpoint
{
    private const string Route = "/question/{id}";

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(Route,
                async ([FromRoute] string id, [FromServices] IDeleteQuestionUseCase handler) =>
                {
                    UniqueEntityId.Of(id);
                    var command = new DeleteQuestionCommand(new UniqueEntityId(id));
                    var result = await handler.DeleteQuestionUseCaseHandler(command);
                    return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
                })
            .WithName("DeleteQuestion")
            .WithTags("Question")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}