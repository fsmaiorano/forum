namespace UnitTests.Domain.Entities;

public class QuestionUnitTest
{
    [Fact]
    public void Create_ShouldCreateQuestionWithValidInputs()
    {
        const string authorId = "author-123";
        const string title = "Sample Title";
        const string content = "Sample Content";
        const string slug = "sample-title";

        var question = Question.Create(authorId, title, content, slug);


        Assert.Equal(authorId, question.AuthorId.ToString());
        Assert.Equal(title, question.Title);
        Assert.Equal(content, question.Content);
        Assert.NotNull(question.Slug);
        Assert.Equal(slug, question.Slug!.Value);
    }

    [Fact]
    public void Create_ShouldCreateQuestionWithNullSlug()
    {
        const string authorId = "author-123";
        const string title = "Sample Title";
        const string content = "Sample Content";

        var question = Question.Create(authorId, title, content);

        Assert.Equal(authorId, question.AuthorId.ToString());
        Assert.Equal(title, question.Title);
        Assert.Equal(content, question.Content);
        Assert.Null(question.Slug);
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