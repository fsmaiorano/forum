using Microsoft.Extensions.Configuration;
using Seed.Models;
using Seed.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>()
    ?? throw new InvalidOperationException("ApiSettings not found in configuration");

var seedSettings = configuration.GetSection("SeedSettings").Get<SeedSettings>()
    ?? throw new InvalidOperationException("SeedSettings not found in configuration");

var userHttpClient = new HttpClient { BaseAddress = new Uri(apiSettings.UserApiBaseUrl) };
var forumHttpClient = new HttpClient { BaseAddress = new Uri(apiSettings.ForumApiBaseUrl) };

var userApiService = new UserApiService(userHttpClient);
var forumApiService = new ForumApiService(forumHttpClient);
var dataGenerator = new DataGenerator();

Console.WriteLine("═══════════════════════════════════════════════════════════");
Console.WriteLine("🚀 Forum Database Seeding Tool");
Console.WriteLine("═══════════════════════════════════════════════════════════");
Console.WriteLine("User API: " + apiSettings.UserApiBaseUrl);
Console.WriteLine("Forum API: " + apiSettings.ForumApiBaseUrl);
Console.WriteLine("Users to create: " + seedSettings.NumberOfUsers.ToString());
Console.WriteLine("Questions per user: " + seedSettings.QuestionsPerUser.ToString());
Console.WriteLine("Answers per question: " + seedSettings.AnswersPerQuestion.ToString());
Console.WriteLine("═══════════════════════════════════════════════════════════\n");

Console.WriteLine("📝 Step 1: Creating Users...");
Console.WriteLine("───────────────────────────────────────────────────────────");

var users = new List<UserInfo>();

for (var i = 0; i < seedSettings.NumberOfUsers; i++)
{
    var registerRequest = dataGenerator.GenerateUser();
    var authResponse = await userApiService.RegisterUserAsync(registerRequest);

    if (authResponse != null)
    {
        var (userId, token) = await userApiService.GetUserIdFromTokenAsync(authResponse.AccessToken);

        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(token))
        {
            users.Add(new UserInfo
            {
                UserId = userId,
                Email = registerRequest.Email,
                Password = registerRequest.Password,
                AccessToken = token
            });
        }
    }

    await Task.Delay(100);
}

Console.WriteLine($"\n✨ Created {users.Count} users successfully\n");

if (users.Count == 0)
{
    Console.WriteLine("❌ No users were created. Cannot proceed with seeding.");
    return;
}

Console.WriteLine("❓ Step 2: Creating Questions...");
Console.WriteLine("───────────────────────────────────────────────────────────");

var allQuestions = new List<QuestionInfo>();

foreach (var user in users)
{
    for (var i = 0; i < seedSettings.QuestionsPerUser; i++)
    {
        var questionRequest = dataGenerator.GenerateQuestion(user.UserId);
        var questionResponse = await forumApiService.CreateQuestionAsync(questionRequest, user.AccessToken);

        if (questionResponse != null)
        {
            allQuestions.Add(new QuestionInfo
            {
                QuestionId = questionResponse.QuestionId,
                AuthorId = user.UserId,
                Title = questionRequest.Title
            });
        }

        await Task.Delay(100);
    }
}

Console.WriteLine($"\n✨ Created {allQuestions.Count} questions successfully\n");

if (allQuestions.Count == 0)
{
    Console.WriteLine("❌ No questions were created. Cannot proceed with seeding.");
    return;
}

Console.WriteLine("💬 Step 3: Creating Answers...");
Console.WriteLine("───────────────────────────────────────────────────────────");

var random = new Random();
var totalAnswersCreated = 0;

foreach (var question in allQuestions)
{
    var eligibleAnswerers = users.Where(u => u.UserId != question.AuthorId).ToList();

    var numberOfAnswers = Math.Min((int)seedSettings.AnswersPerQuestion, eligibleAnswerers.Count);
    var selectedAnswerers = eligibleAnswerers.OrderBy(_ => random.Next()).Take(numberOfAnswers).ToList();

    foreach (var answerer in selectedAnswerers)
    {
        var answerRequest = dataGenerator.GenerateAnswer(question.QuestionId, answerer.UserId, question.Title);
        var answerResponse = await forumApiService.CreateAnswerAsync(answerRequest, answerer.AccessToken);

        if (answerResponse != null)
        {
            question.Answers.Add(new AnswerInfo
            {
                AnswerId = answerResponse.AnswerId,
                AuthorId = answerer.UserId,
                QuestionId = question.QuestionId
            });
            totalAnswersCreated++;
        }

        await Task.Delay(100);
    }
}

Console.WriteLine($"\n✨ Created {totalAnswersCreated} answers successfully\n");

Console.WriteLine("⭐ Step 4: Marking Best Answers...");
Console.WriteLine("───────────────────────────────────────────────────────────");

var bestAnswersMarked = 0;

foreach (var question in allQuestions.Where(q => q.Answers.Any()))
{
    if (random.NextDouble() <= seedSettings.BestAnswerPercentage)
    {
        var questionAuthor = users.FirstOrDefault(u => u.UserId == question.AuthorId);
        if (questionAuthor == null) continue;

        var bestAnswer = question.Answers[random.Next(question.Answers.Count)];

        var success = await forumApiService.SetBestAnswerAsync(
            question.QuestionId,
            question.AuthorId,
            bestAnswer.AnswerId,
            questionAuthor.AccessToken);

        if (success)
            bestAnswersMarked++;

        await Task.Delay(100);
    }
}

Console.WriteLine($"\n✨ Marked {bestAnswersMarked} best answers successfully\n");

Console.WriteLine("═══════════════════════════════════════════════════════════");
Console.WriteLine("📊 SEEDING SUMMARY");
Console.WriteLine("═══════════════════════════════════════════════════════════");
Console.WriteLine($"👥 Users created: {users.Count}");
Console.WriteLine($"❓ Questions created: {allQuestions.Count}");
Console.WriteLine($"💬 Answers created: {totalAnswersCreated}");
Console.WriteLine($"⭐ Best answers marked: {bestAnswersMarked}");
Console.WriteLine("═══════════════════════════════════════════════════════════");
Console.WriteLine();

Console.WriteLine("📋 Sample Created Data:");
Console.WriteLine("───────────────────────────────────────────────────────────");
Console.WriteLine("\n👤 Sample Users:");
foreach (var user in users.Take(3))
    Console.WriteLine($"   • {user.Email} (ID: {user.UserId[..8]}...)");

Console.WriteLine("\n❓ Sample Questions:");
foreach (var question in allQuestions.Take(3))
{
    Console.WriteLine($"   • {question.Title}");
    Console.WriteLine($"     ID: {question.QuestionId[..8]}... | Answers: {question.Answers.Count}");
}

Console.WriteLine();
Console.WriteLine("✅ Database seeding completed successfully!");
Console.WriteLine("═══════════════════════════════════════════════════════════\n");