namespace Forum.BuildingBlocks.Base;

public abstract record Entity
{
    protected UniqueEntityId _id = null!;
    private DateTime CreatedAt { get; set; }
    private DateTime? UpdatedAt { get; set; }

    public UniqueEntityId Id 
    { 
        get => _id ??= new UniqueEntityId();
        protected set => _id = value;
    }
    
    public void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}