namespace Seed.Models;

public class ApiSettings
{
    public string UserApiBaseUrl { get; set; } = string.Empty;
    public string ForumApiBaseUrl { get; set; } = string.Empty;
}

public class SeedSettings
{
    public int NumberOfUsers { get; set; }
    public int QuestionsPerUser { get; set; }
    public int AnswersPerQuestion { get; set; }
    public double BestAnswerPercentage { get; set; }
}

