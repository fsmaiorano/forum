using Forum.Application.UseCases.Answer.GetAnswers;
using Forum.Endpoints.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Endpoints.Answer;

public record GetAnswersResponse(List<AnswerDto> Answers);

public static class GetAnswersEndpoint
{
    private const string Route = "/answer/{questionId}";

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Route,
                async (string questionId, [FromServices] IGetAnswersUseCase handler) =>
                {
                    var result = await handler.GetAnswersUseCaseHandler(questionId);

                    if (result.IsSuccess && !result.Value.Answers.Any())
                        return Results.NoContent();

                    var answerListDto = result.Value.Answers.Select(answerEntity => new AnswerDto()
                        {
                            Id = answerEntity.Id.ToString(),
                            AuthorId = answerEntity.AuthorId.ToString(),
                            QuestionId = answerEntity.QuestionId.ToString(),
                            Content = answerEntity.Content,
                            Attachments = answerEntity.Attachments.CurrentItems.Select(a => new AttachmentDto
                            {
                                Id = a.Id.ToString(), OwnerId = a.OwnerId.ToString(), Link = a.Link, Title = a.Title
                            })
                        })
                        .ToList();

                    return result.IsSuccess
                        ? Results.Ok(new GetAnswersResponse(answerListDto))
                        : Results.BadRequest(result.Error);
                })
            .WithName("GetAnswers")
            .WithTags("Answer")
            .Produces<GetAnswersResponse>(StatusCodes.Status200OK)
            .Produces<GetAnswersResponse>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}