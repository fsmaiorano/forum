using Forum.BuildingBlocks.Base;

namespace Forum.Domain.Entities;

public sealed record Question : Entity
{
    public UniqueEntityId AuthorId { get; private set; } = null!;
    public UniqueEntityId BestAnswerId { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public Slug? Slug { get; private set; } = null;
    public IEnumerable<Attachment> Attachments { get; private set; } = [];

    public static Question Create(string authorId, string title, string content, string? slug = null)
    {
        var question = new Question()
        {
            Id = new UniqueEntityId(),
            AuthorId = new UniqueEntityId(authorId),
            Title = title,
            Content = content,
            Slug = string.IsNullOrWhiteSpace(slug) ? Slug.Create(title) : new Slug(slug),
        };

        return question;
    }

    public static Question Update(Question question,string title, string content, string? slug = null)
    {
        question.Title = title;
        question.Content = content;
        question.Slug = string.IsNullOrWhiteSpace(slug) ? Slug.Create(title) : new Slug(slug);
  
        return question;
    }

    public static string Excerpt(string content, int maxLength = 200)
    {
        if (string.IsNullOrEmpty(content))
            return string.Empty;

        return content.Length <= maxLength ? content : string.Concat(content.AsSpan(0, maxLength), "...");
    }
}