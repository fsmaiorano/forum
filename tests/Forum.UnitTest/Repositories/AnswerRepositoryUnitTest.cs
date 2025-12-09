using Forum.UnitTest.Base;
using Forum.UnitTest.Factories;
using Forum.UnitTest.Fixtures;

namespace Forum.UnitTest.Repositories;

public class AnswerRepositoryUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Create_ShouldAddAnswerToQuestion()
    {
        var answerRepository = new AnswerRepository(Context, DomainEventDispatcher);
        var questionRepository = new QuestionRepository(Context, DomainEventDispatcher);

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
        var answerRepository = new AnswerRepository(Context, DomainEventDispatcher);
        var questionRepository = new QuestionRepository(Context, DomainEventDispatcher);

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
        var answerRepository = new AnswerRepository(Context, DomainEventDispatcher);
        var questionRepository = new QuestionRepository(Context, DomainEventDispatcher);

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
        var answerRepository = new AnswerRepository(Context, DomainEventDispatcher);
        var questionRepository = new QuestionRepository(Context, DomainEventDispatcher);

        var question = MakeQuestion.Create();
        await questionRepository.Create(question);

        var answer = MakeAnswer.Create(questionId: question.Id);
        await answerRepository.Create(answer);

        await answerRepository.Delete(answer);

        var storedAnswer = await Context.Answer.FirstOrDefaultAsync(a => a.Id == answer.Id);

        Assert.Null(storedAnswer);
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnAllAnswersByQuestionId()
    {
        var answerRepository = new AnswerRepository(Context, DomainEventDispatcher);
        var questionRepository = new QuestionRepository(Context, DomainEventDispatcher);

        var question = MakeQuestion.Create();
        await questionRepository.Create(question);

        var answer1 = MakeAnswer.Create(questionId: question.Id);
        var answer2 = MakeAnswer.Create(questionId: question.Id);
        await answerRepository.Create(answer1);
        await answerRepository.Create(answer2);

        var allAnswers = await answerRepository.GetByQuestionId(question.Id);

        var answerEntities = allAnswers.ToList();
        Assert.Contains(answerEntities, a => a.Id == answer1.Id);
        Assert.Contains(answerEntities, a => a.Id == answer2.Id);
    }
}