using UnitTests.Factories;

namespace UnitTests.Domain.Entities;

public class QuestionUnitTest
{
    [Fact]
    public void Create_ShouldCreateQuestionWithValidInputs()
    {
        var mock = MakeQuestion.Create();

        var question = Question.Create(
            mock.AuthorId.ToString(),
            mock.Title,
            mock.Content,
            mock.Slug?.Value);

        Assert.Equal(mock.AuthorId.ToString(), question.AuthorId.ToString());
        Assert.Equal(mock.Title, question.Title);
        Assert.Equal(mock.Content, question.Content);
        Assert.Equal(mock.Slug?.Value, question.Slug?.Value);
    }

    [Fact]
    public void Excerpt_ShouldReturnFullContentIfWithinMaxLength()
    {
        const string content = "Short content";

        var result = Question.Excerpt(content, 50);

        Assert.Equal(content, result);
    }

    [Fact]
    public void Excerpt_ShouldReturnEmptyStringForNullContent()
    {
        var result = Question.Excerpt(null!);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Excerpt_ShouldReturnEmptyStringForEmptyContent()
    {
        var result = Question.Excerpt(string.Empty);
        Assert.Equal(string.Empty, result);
    }
}