using UnitTests.Factories;

namespace UnitTests.Repositories;

public class AnswerRepositoryUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Create_ShouldAddAnswerToQuestion()
    {
        var answerRepository = new AnswerRepository(Context);
        var questionRepository = new QuestionRepository(Context);
        
        var question = MakeQuestion.Create();
        await questionRepository.Create(question);
        
        var answer = MakeAnswer.Create(questionId: question.Id);
        await answerRepository.Create(answer);
        
        var storedAnswer = await Context.Answer.FirstOrDefaultAsync(a => a.Id == answer.Id);
        
        Assert.NotNull(storedAnswer);
        Assert.Equal(answer.Content, storedAnswer.Content);
        Assert.Equal(answer.QuestionId, storedAnswer.QuestionId);
    }

    [Fact]
    public async Task FindByQuestionId_ShouldReturnAnswersByQuestionId()
    {
        var answerRepository = new AnswerRepository(Context);
        var questionRepository = new QuestionRepository(Context);
        
        var question = MakeQuestion.Create();
        await questionRepository.Create(question);
        
        var answer = MakeAnswer.Create(questionId: question.Id);
        await answerRepository.Create(answer);
        
        var storedAnswer = await answerRepository.FindById(answer.Id);
        
        Assert.NotNull(storedAnswer);
        Assert.Equal(answer.Content, storedAnswer.Content);
        Assert.Equal(answer.QuestionId, storedAnswer.QuestionId);
    }

    [Fact]
    public async Task Update_ShouldUpdateAnswer()
    {
        var answerRepository = new AnswerRepository(Context);
        var questionRepository = new QuestionRepository(Context);
        
        var question = MakeQuestion.Create();
        await questionRepository.Create(question);
        
        var answer = MakeAnswer.Create(questionId: question.Id);
        await answerRepository.Create(answer);
        
        answer.Touch();
        await answerRepository.Update(answer);
        
        var storedAnswer = await Context.Answer.FirstOrDefaultAsync(a => a.Id == answer.Id);
        
        Assert.NotNull(storedAnswer);
        Assert.Equal(answer.Content, storedAnswer.Content);
        Assert.Equal(answer.QuestionId, storedAnswer.QuestionId);
        Assert.NotNull(answer.UpdatedAt);
    }
    
    [Fact]
    public async Task Delete_ShouldDeleteAnswer()
    {
        var answerRepository = new AnswerRepository(Context);
        var questionRepository = new QuestionRepository(Context);
        
        var question = MakeQuestion.Create();
        await questionRepository.Create(question);
        
        var answer = MakeAnswer.Create(questionId: question.Id);
    }
}

