using Forum.Application.UseCases.Question.GetQuestions;
using Forum.Endpoints.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Endpoints.Question;

public record GetQuestionsResponse(IEnumerable<QuestionDto> Questions);

public static class GetQuestionsEndpoint
{
    private const string Route = "/question/{id}";

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Route,
                async ([FromRoute] string id, [FromServices] IGetQuestionsUseCase handler) =>
                {
                    var result = await handler.GetQuestionsUseCaseHandler();

                    if (result.IsSuccess && !result.Value.Questions.Any())
                        return Results.NoContent();

                    var questionListDto = result.Value.Questions.Select(questionEntity => new QuestionDto
                        {
                            Id = questionEntity.Id.ToString(),
                            AuthorId = questionEntity.AuthorId.ToString(),
                            BestAnswerId = questionEntity.BestAnswerId?.ToString(),
                            Title = questionEntity.Title,
                            Content = questionEntity.Content,
                            Slug = questionEntity.Slug?.Value ?? string.Empty,
                            IsOpen = questionEntity.IsOpen,
                            Attachments = questionEntity.Attachments?.CurrentItems.Select(a => new AttachmentDto
                            {
                                Id = a.Id.ToString(), OwnerId = a.OwnerId.ToString(), Link = a.Link, Title = a.Title
                            })
                        })
                        .ToList();

                    return result.IsSuccess
                        ? Results.Ok(new GetQuestionsResponse(questionListDto))
                        : Results.BadRequest(result.Error);
                })
            .WithName("GetQuestions")
            .WithTags("Question")
            .Produces<GetQuestionsResponse>(StatusCodes.Status200OK)
            .Produces<GetQuestionsResponse>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}