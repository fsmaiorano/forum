using Forum.BuildingBlocks.Base;

namespace Forum.Domain.Entities;

public sealed record StudentEntity : Entity
{
    public string Name { get; private set; } = string.Empty;

    public static StudentEntity Create(string name)
    {
        return new StudentEntity
        {
            Name = name
        };
    }
}