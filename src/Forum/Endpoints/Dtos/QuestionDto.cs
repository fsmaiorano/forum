namespace Forum.Endpoints.Dtos;

public record QuestionDto
{
    public string? Id { get; set; }
    public string? AuthorId { get; set; }
    public string? BestAnswerId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Slug { get; set; }
    public IEnumerable<AttachmentDto>? Attachments { get; set; } = [];
    public bool IsOpen { get; set; }
}