namespace Forum.Domain.Entities.Generic;

public sealed class UniqueEntityId : IEquatable<UniqueEntityId>
{
    private readonly Guid _value;

    public UniqueEntityId(string? value = null)
    {
        _value = value is null
            ? Guid.NewGuid()
            : Guid.TryParse(value, out var guidValue)
                ? guidValue
                : throw new ArgumentException("Invalid GUID format.", nameof(value));
    }

    private UniqueEntityId(Guid value)
    {
        _value = value;
    }

    public override string ToString() => _value.ToString();

    public Guid ToGuid() => _value;

    /// <summary>
    /// Creates a new UniqueEntityId from a string value.
    /// </summary>
    public static UniqueEntityId Of(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("UniqueEntityId cannot be null or empty.", nameof(value))
            : new UniqueEntityId(value);
    }
    
    /// <summary>
    /// Creates a new UniqueEntityId from a Guid value.
    /// </summary>
    public static UniqueEntityId Of(Guid value)
    {
        return new UniqueEntityId(value);
    }

    /// <summary>
    /// Creates a new UniqueEntityId with a new Guid.
    /// </summary>
    public static UniqueEntityId NewId()
    {
        return new UniqueEntityId(Guid.NewGuid());
    }

    #region Equality Members

    public bool Equals(UniqueEntityId? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _value.Equals(other._value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is UniqueEntityId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static bool operator ==(UniqueEntityId? left, UniqueEntityId? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(UniqueEntityId? left, UniqueEntityId? right)
    {
        return !(left == right);
    }

    #endregion

    #region Conversion Operators

    /// <summary>
    /// Implicit conversion from UniqueEntityId to Guid.
    /// </summary>
    public static implicit operator Guid(UniqueEntityId id) => id._value;

    /// <summary>
    /// Explicit conversion from Guid to UniqueEntityId.
    /// </summary>
    public static explicit operator UniqueEntityId(Guid value) => new(value);

    /// <summary>
    /// Explicit conversion from string to UniqueEntityId.
    /// </summary>
    public static explicit operator UniqueEntityId(string value) => Of(value);

    #endregion
}