using System.Collections;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;

namespace UnitTests.Fixtures;

public sealed class HttpFixture : WebApplicationFixture
{
    private HttpClient? _httpClient;

    private HttpClient GetHttpClient()
    {
        return _httpClient ??= CreateClient();
    }

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

        MultipartFormDataContent multipartContent = new MultipartFormDataContent();

        List<PropertyInfo> requestProperties = request.GetType().GetProperties().ToList();

        foreach (PropertyInfo property in requestProperties)
        {
            object? propertyValue = property.GetValue(request);

            if (string.IsNullOrWhiteSpace(propertyValue?.ToString()))
                continue;

            if (propertyValue is IList list)
            {
                AddListToMultipartContent(multipartContent, property.Name, list);
            }
            else
            {
                multipartContent.Add(new StringContent(propertyValue.ToString()!), property.Name);
            }
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
        Type itemType = list.GetType().GetGenericArguments().Single();

        if (itemType.IsClass && itemType != typeof(string))
        {
            AddClassListToMultipartContent(multipartContent, propertyName, list);
        }
        else
        {
            foreach (object? item in list)
            {
                multipartContent.Add(new StringContent(item.ToString()!), propertyName);
            }
        }
    }

    private static void AddClassListToMultipartContent(
        MultipartFormDataContent multipartContent,
        string propertyName,
        IList list)
    {
        int index = 0;

        foreach (object? item in list)
        {
            List<PropertyInfo> classPropertiesInfo = item.GetType().GetProperties().ToList();

            foreach (PropertyInfo prop in classPropertiesInfo)
            {
                object? value = prop.GetValue(item, null);
                multipartContent.Add(new StringContent(value!.ToString()!), $"{propertyName}[{index}][{prop.Name}]");
            }

            index++;
        }
    }
}