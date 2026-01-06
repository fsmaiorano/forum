using System.Net.Http.Json;
using System.Text.Json;
using Seed.Models;

namespace Seed.Services;

public class UserApiService(HttpClient httpClient)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<AuthResponse?> RegisterUserAsync(RegisterRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("/api/auth/register", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Failed to register user {request.Email}: {response.StatusCode}");
                Console.WriteLine($"   Error: {errorContent}");
                return null;
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);
            Console.WriteLine($"✅ User registered: {request.Email}");
            return authResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception registering user {request.Email}: {ex.Message}");
            return null;
        }
    }

    public async Task<(string? userId, string? token)> GetUserIdFromTokenAsync(string accessToken)
    {
        try
        {
            httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            var response = await httpClient.GetAsync("/api/auth/me");
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Failed to get user info: {response.StatusCode}");
                return (null, null);
            }

            var userInfo = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(_jsonOptions);
            if (userInfo != null && userInfo.TryGetValue("userId", out var userId))
                return (userId.ToString(), accessToken);

            return (null, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception getting user info: {ex.Message}");
            return (null, null);
        }
    }
}

