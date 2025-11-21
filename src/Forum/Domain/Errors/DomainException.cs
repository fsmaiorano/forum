namespace Forum.Domain.Errors;

public class DomainException(string message) : Exception(message);
