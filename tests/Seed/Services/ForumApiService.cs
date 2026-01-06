using System.Net.Http.Json;
using System.Text.Json;
using Seed.Models;

namespace Seed.Services;

public class ForumApiService(HttpClient httpClient)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<CreateQuestionResponse?> CreateQuestionAsync(CreateQuestionRequest request, string? accessToken = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await httpClient.PostAsJsonAsync("/question", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Failed to create question: {response.StatusCode}");
                Console.WriteLine($"   Title: {request.Title}");
                Console.WriteLine($"   Error: {errorContent}");
                return null;
            }

            var questionResponse = await response.Content.ReadFromJsonAsync<CreateQuestionResponse>(_jsonOptions);
            Console.WriteLine($"✅ Question created: {request.Title[..Math.Min(50, request.Title.Length)]}...");
            return questionResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception creating question: {ex.Message}");
            return null;
        }
    }

    public async Task<CreateAnswerResponse?> CreateAnswerAsync(CreateAnswerRequest request, string? accessToken = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(accessToken))
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.PostAsJsonAsync("/answer", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Failed to create answer: {response.StatusCode}");
                Console.WriteLine($"   Error: {errorContent}");
                return null;
            }

            var answerResponse = await response.Content.ReadFromJsonAsync<CreateAnswerResponse>(_jsonOptions);
            Console.WriteLine($"✅ Answer created for question {request.QuestionId[..8]}...");
            return answerResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception creating answer: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> SetBestAnswerAsync(string questionId, string authorId, string answerId, string? accessToken = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(accessToken))
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var request = new PatchQuestionSetBestAnswerRequest
            {
                QuestionId = questionId,
                AuthorId = authorId,
                AnswerId = answerId
            };

            var response = await httpClient.PatchAsJsonAsync($"/question/{questionId}/best-answer", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Failed to set best answer: {response.StatusCode}");
                Console.WriteLine($"   Error: {errorContent}");
                return false;
            }

            Console.WriteLine($"✅ Best answer set for question {questionId[..8]}...");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception setting best answer: {ex.Message}");
            return false;
        }
    }
}

