using System.ComponentModel.DataAnnotations;

namespace Forum.Domain.Entities.ValuesObjects;

public sealed record Slug(string Value)
{
    [MaxLength(200)]
    public string Value { get; private set; } = Value;

    public static Slug CreateFromText(string text)
    {
        var slugValue = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in slugValue.Where(c =>
                     System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) !=
                     System.Globalization.UnicodeCategory.NonSpacingMark))
        {
            stringBuilder.Append(c);
        }

        slugValue = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        slugValue = slugValue.ToLowerInvariant().Trim();
        slugValue = Regex.Replace(slugValue, @"[^a-z0-9]+", "-", RegexOptions.None, TimeSpan.FromMilliseconds(100));
        slugValue = Regex.Replace(slugValue, @"^-+|-+$", "", RegexOptions.None, TimeSpan.FromMilliseconds(100));

        return new Slug(slugValue);
    }

    public static Slug Create(string slug)
    {
        return new Slug(slug);
    }
}