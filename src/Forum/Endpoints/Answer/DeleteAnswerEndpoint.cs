using Forum.Application.UseCases.Answer.DeleteAnswer;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Endpoints.Answer;

public record DeleteAnswerRequest();

// public record DeleteAnswerResponse();

public class DeleteAnswerEndpoint
{
    private const string Route = "/answer/{id}";

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(Route,
                async ([FromRoute] string id, [FromServices] IDeleteAnswerUseCase handler) =>
                {
                    UniqueEntityId.Of(id);
                    var command = new DeleteAnswerCommand(new UniqueEntityId(id));
                    var result = await handler.DeleteAnswerUseCaseHandler(command);
                    return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
                })
            .WithName("DeleteAnswer")
            .WithTags("Answer")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}