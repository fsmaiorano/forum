namespace Forum.Endpoints.Dtos;

public record AnswerDto
{
    public string? Id { get; set; }
    public string? AuthorId { get; set; }
    public string? QuestionId { get; set; }
    public string? Content { get; set; }
    public IEnumerable<AttachmentDto> Attachments { get; set; } = [];
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}