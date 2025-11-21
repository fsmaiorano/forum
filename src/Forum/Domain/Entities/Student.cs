namespace Forum.Domain.Entities;

public record Student : Entity
{
    public string Name { get; private set; } = string.Empty;

    public static Student Create(string name)
    {
        return new Student
        {
            Name = name
        };
    }
}