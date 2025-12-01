namespace BuildingBlocks.Extensions;

public static class StringExtensions
{
    public static string Excerpt(this string content, int maxLength = 200)
    {
        if (string.IsNullOrEmpty(content))
            return string.Empty;

        return content.Length <= maxLength ? content : string.Concat(content.AsSpan(0, maxLength), "...");
    }
}