namespace Seed.Models;

public class CreateQuestionRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public List<AttachmentRequest>? Attachments { get; set; }
}

public class CreateQuestionResponse
{
    public string QuestionId { get; set; } = string.Empty;
}

public class CreateAnswerRequest
{
    public string QuestionId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<AttachmentRequest>? Attachments { get; set; }
}

public class CreateAnswerResponse
{
    public string AnswerId { get; set; } = string.Empty;
}

public class PatchQuestionSetBestAnswerRequest
{
    public string QuestionId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AnswerId { get; set; } = string.Empty;
}

public class AttachmentRequest
{
    public string OwnerId { get; set; } = string.Empty;
    public int OwnerType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
}

public class QuestionInfo
{
    public string QuestionId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<AnswerInfo> Answers { get; set; } = new();
}

public class AnswerInfo
{
    public string AnswerId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
}

