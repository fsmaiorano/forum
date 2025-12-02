using System.Collections;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Forum.Notification.Fixtures;

public sealed class TestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string DatabaseName = "NotificationInMemoryTestDb";
    private HttpClient? _httpClient;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<NotificationDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            var interfaceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(INotificationDbContext));

            if (interfaceDescriptor != null)
                services.Remove(interfaceDescriptor);

            services.AddDbContext<NotificationDbContext>(options =>
                options.UseInMemoryDatabase(DatabaseName));

            services.AddScoped<INotificationDbContext>(provider =>
                provider.GetRequiredService<NotificationDbContext>());
        });

        base.ConfigureWebHost(builder);
    }

    public NotificationDbContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    }

    private HttpClient GetHttpClient()
    {
        return _httpClient ??= CreateClient();
    }

    #region HTTP Helper Methods

    public async Task<HttpResponseMessage> DoPost(string method, object request, string token = "",
        string culture = "en-US")
    {
        var client = GetHttpClient();
        ChangeRequestCulture(culture, client);
        AuthorizeRequest(token, client);
        return await client.PostAsJsonAsync(method, request);
    }

    public async Task<HttpResponseMessage> DoGet(string method, string token = "", string culture = "en-US")
    {
        var client = GetHttpClient();
        ChangeRequestCulture(culture, client);
        AuthorizeRequest(token, client);
        return await client.GetAsync(method);
    }

    public async Task<HttpResponseMessage> DoPut(string method, object request, string token = "",
        string culture = "en-US")
    {
        var client = GetHttpClient();
        ChangeRequestCulture(culture, client);
        AuthorizeRequest(token, client);
        return await client.PutAsJsonAsync(method, request);
    }

    public async Task<HttpResponseMessage> DoPatch(string method, object request, string token = "",
        string culture = "en-US")
    {
        var client = GetHttpClient();
        ChangeRequestCulture(culture, client);
        AuthorizeRequest(token, client);
        return await client.PatchAsJsonAsync(method, request);
    }

    public async Task<HttpResponseMessage> DoPostFormData(
        string method,
        object request,
        string token,
        string culture = "en-US")
    {
        var client = GetHttpClient();
        ChangeRequestCulture(culture, client);
        AuthorizeRequest(token, client);

        var multipartContent = new MultipartFormDataContent();

        var requestProperties = request.GetType().GetProperties().ToList();

        foreach (var property in requestProperties)
        {
            var propertyValue = property.GetValue(request);

            if (string.IsNullOrWhiteSpace(propertyValue?.ToString()))
                continue;

            if (propertyValue is IList list)
                AddListToMultipartContent(multipartContent, property.Name, list);
            else
                multipartContent.Add(new StringContent(propertyValue.ToString()!), property.Name);
        }

        return await client.PostAsync(method, multipartContent);
    }

    public async Task<HttpResponseMessage> DoDelete(string method, string token = "", string culture = "en-US")
    {
        var client = GetHttpClient();
        ChangeRequestCulture(culture, client);
        AuthorizeRequest(token, client);

        return await client.DeleteAsync(method);
    }

    #endregion

    #region Private Helper Methods

    private void ChangeRequestCulture(string culture, HttpClient client)
    {
        if (client.DefaultRequestHeaders.Contains("Accept-Language"))
            client.DefaultRequestHeaders.Remove("Accept-Language");

        client.DefaultRequestHeaders.Add("Accept-Language", culture);
    }

    private void AuthorizeRequest(string token, HttpClient client)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static void AddListToMultipartContent(
        MultipartFormDataContent multipartContent,
        string propertyName,
        IList list)
    {
        var itemType = list.GetType().GetGenericArguments().Single();

        if (itemType.IsClass && itemType != typeof(string))
            AddClassListToMultipartContent(multipartContent, propertyName, list);
        else
            foreach (var item in list)
                multipartContent.Add(new StringContent(item.ToString()!), propertyName);
    }

    private static void AddClassListToMultipartContent(
        MultipartFormDataContent multipartContent,
        string propertyName,
        IList list)
    {
        var index = 0;

        foreach (object? item in list)
        {
            var classPropertiesInfo = item.GetType().GetProperties().ToList();
            foreach (var prop in classPropertiesInfo)
            {
                var value = prop.GetValue(item, null);
                multipartContent.Add(new StringContent(value!.ToString()!), $"{propertyName}[{index}][{prop.Name}]");
            }

            index++;
        }
    }

    #endregion

    #region IAsyncLifetime Implementation

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public new Task DisposeAsync()
    {
        return base.DisposeAsync().AsTask();
    }

    #endregion
}

