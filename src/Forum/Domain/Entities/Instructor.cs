using Forum.BuildingBlocks.Base;

namespace Forum.Domain.Entities;

public sealed record Instructor : Entity
{
    public string Name { get; private set; } = string.Empty;
    
    public static Instructor Create(string name)
    {
        return new Instructor
        {
            Name = name
        };
    }
}