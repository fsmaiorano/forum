namespace BuildingBlocks.Base;

public abstract record Entity
{
    protected UniqueEntityId _id = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

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