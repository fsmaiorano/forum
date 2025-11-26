namespace Forum.Endpoints.Dtos;

public abstract record AttachmentRequest(string OwnerId, int OwnerType, string Title, string Link);