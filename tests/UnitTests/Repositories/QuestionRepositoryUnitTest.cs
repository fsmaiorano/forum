using UnitTests.Factories;

namespace UnitTests.Repositories;

public class QuestionRepositoryUnitTest(DatabaseFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Create_ShouldAddQuestionToContext()
    {
        var repository = new QuestionRepository(Context);

        var question = MakeQuestion.Create();
        await repository.Create(question);
        await Context.SaveChangesAsync();

        var storedQuestion = await Context.Question
            .FirstOrDefaultAsync(q => q.Id == question.Id);

        Assert.NotNull(storedQuestion);
        Assert.Equal(question.Title, storedQuestion.Title);
        Assert.Equal(question.Content, storedQuestion.Content);
    }

    [Fact]
    public async Task FindById_ShouldReturnQuestion_WhenQuestionExists()
    {
        var repository = new QuestionRepository(Context);   
        
        var question = MakeQuestion.Create();
        await repository.Create(question);
        
        var storedQuestion = await repository.FindById(question.Id.ToString());
        
        Assert.NotNull(storedQuestion);
    }
    
    [Fact]
    public async Task FindById_ShouldReturnNull_WhenQuestionDoesNotExist()
    {
        var repository = new QuestionRepository(Context);   
        
        var question = await repository.FindById("123");
        
        Assert.Null(question);
    }
    
    [Fact]
    public async Task Update_ShouldUpdateAndSaveQuestion()
    {
        var repository = new QuestionRepository(Context);   
        
        var question = MakeQuestion.Create();
        await repository.Create(question);
        
        question.Touch();
        await repository.Update(question);
        
        var storedQuestion = await Context.Question
            .FirstOrDefaultAsync(q => q.Id == question.Id);
        
        Assert.NotNull(storedQuestion);
        Assert.Equal(question.Title, storedQuestion.Title);
        Assert.Equal(question.Content, storedQuestion.Content);
        Assert.NotNull(storedQuestion.UpdatedAt);
    }
}