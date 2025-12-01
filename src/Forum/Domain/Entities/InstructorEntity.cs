using BuildingBlocks.Base;

namespace Forum.Domain.Entities;

public sealed record InstructorEntity : Entity
{
    public string Name { get; private set; } = string.Empty;
    
    public static InstructorEntity Create(string name)
    {
        return new InstructorEntity
        {
            Name = name
        };
    }
}