namespace Forum.Endpoints.Dtos;

public record AttachmentRequest(string OwnerId, int OwnerType, string Title, string Link);
