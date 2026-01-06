using Bogus;
using Seed.Models;

namespace Seed.Services;

public class DataGenerator
{
    private readonly Faker _faker = new();
    private readonly Random _random = new();

    public RegisterRequest GenerateUser()
    {
        var firstName = _faker.Name.FirstName();
        var lastName = _faker.Name.LastName();
        var email = _faker.Internet.Email(firstName, lastName).ToLower();
        
        return new RegisterRequest
        {
            Email = email,
            Password = "Test@123" // Using a standard password for testing
        };
    }

    public CreateQuestionRequest GenerateQuestion(string authorId)
    {
        var topics = new[]
        {
            "ASP.NET Core", "Entity Framework", "Blazor", "SignalR", "REST API",
            "Authentication", "Authorization", "Docker", "Microservices", "CQRS",
            "Clean Architecture", "Design Patterns", "Performance", "Security", "Testing",
            "Dependency Injection", "Middleware", "Logging", "Configuration", "Database"
        };

        var questionTypes = new[]
        {
            "How to implement {0} in {1}?",
            "Best practices for {0} with {1}",
            "What is the difference between {0} and {1}?",
            "How do I optimize {0} for {1}?",
            "Common mistakes when using {0} in {1}",
            "Getting started with {0} in {1}",
            "Troubleshooting {0} issues in {1}",
            "Advanced techniques for {0} in {1}",
            "How to integrate {0} with {1}?",
            "Understanding {0} concepts in {1}"
        };

        var topic1 = topics[_random.Next(topics.Length)];
        var topic2 = topics[_random.Next(topics.Length)];
        var questionTemplate = questionTypes[_random.Next(questionTypes.Length)];
        var title = string.Format(questionTemplate, topic1, topic2);

        var content = GenerateQuestionContent(topic1, topic2);

        return new CreateQuestionRequest
        {
            Title = title,
            Content = content,
            AuthorId = authorId
        };
    }

    private string GenerateQuestionContent(string topic1, string topic2)
    {
        var contexts = new[]
        {
            "I'm working on a project that involves " + topic1 + " and " + topic2 + ". ",
            "I've been learning about " + topic1 + " and trying to integrate it with " + topic2 + ". ",
            "Our team is evaluating " + topic1 + " for use with " + topic2 + ". ",
            "I'm building an application using " + topic1 + " and " + topic2 + ". "
        };

        var problems = new[]
        {
            "I'm encountering some issues and could use some guidance.",
            "I'm not sure about the best approach to take.",
            "I want to ensure I'm following best practices.",
            "I'm looking for advice from experienced developers.",
            "I need help understanding the proper implementation."
        };

        var details = new[]
        {
            "Specifically, I'm having trouble with " + _faker.Lorem.Sentence(),
            "I've tried several approaches but " + _faker.Lorem.Sentence(),
            "The documentation mentions " + _faker.Lorem.Sentence(),
            "I've read that " + _faker.Lorem.Sentence()
        };

        return contexts[_random.Next(contexts.Length)] +
               problems[_random.Next(problems.Length)] + " " +
               details[_random.Next(details.Length)] + "\n\n" +
               "Any suggestions or resources would be greatly appreciated!";
    }

    public CreateAnswerRequest GenerateAnswer(string questionId, string authorId, string questionTitle)
    {
        var answerStyles = new[]
        {
            GenerateDetailedAnswer,
            GenerateCodeBasedAnswer,
            GenerateLinkAnswer,
            GenerateExperienceAnswer,
            GenerateQuickTipAnswer
        };

        var selectedStyle = answerStyles[_random.Next(answerStyles.Length)];
        var content = selectedStyle(questionTitle);

        return new CreateAnswerRequest
        {
            QuestionId = questionId,
            AuthorId = authorId,
            Content = content
        };
    }

    private string GenerateDetailedAnswer(string questionTitle)
    {
        return "Great question! Let me provide a detailed explanation.\n\n" +
               _faker.Lorem.Paragraph(3) + "\n\n" +
               "Here are the key points to consider:\n" +
               "1. " + _faker.Lorem.Sentence() + "\n" +
               "2. " + _faker.Lorem.Sentence() + "\n" +
               "3. " + _faker.Lorem.Sentence() + "\n\n" +
               "Hope this helps! Let me know if you need any clarification.";
    }

    private string GenerateCodeBasedAnswer(string questionTitle)
    {
        return "Here's an example implementation:\n\n" +
               "```csharp\n" +
               "public class Example\n" +
               "{\n" +
               "    // " + _faker.Lorem.Sentence() + "\n" +
               "    public async Task<Result> ProcessAsync()\n" +
               "    {\n" +
               "        // " + _faker.Lorem.Sentence() + "\n" +
               "        return await Task.FromResult(Result.Success());\n" +
               "    }\n" +
               "}\n" +
               "```\n\n" +
               _faker.Lorem.Sentence();
    }

    private string GenerateLinkAnswer(string questionTitle)
    {
        return "I've dealt with this before. Here are some helpful resources:\n\n" +
               "- Microsoft Documentation: " + _faker.Internet.Url() + "\n" +
               "- Blog post: " + _faker.Internet.Url() + "\n" +
               "- Stack Overflow discussion: " + _faker.Internet.Url() + "\n\n" +
               _faker.Lorem.Paragraph();
    }

    private string GenerateExperienceAnswer(string questionTitle)
    {
        return "In my experience working with this, " + _faker.Lorem.Paragraph() + "\n\n" +
               "I've found that the best approach is to " + _faker.Lorem.Sentence() + " " +
               "Additionally, " + _faker.Lorem.Sentence() + "\n\n" +
               "One thing to watch out for is " + _faker.Lorem.Sentence();
    }

    private string GenerateQuickTipAnswer(string questionTitle)
    {
        return "Quick tip: " + _faker.Lorem.Sentence() + "\n\n" +
               "You can achieve this by " + _faker.Lorem.Paragraph() + "\n\n" +
               "Good luck!";
    }
}

