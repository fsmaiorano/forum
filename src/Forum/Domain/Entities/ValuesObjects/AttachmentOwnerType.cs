using BuildingBlocks.Base;
using Forum.Domain.Enums;

namespace Forum.Domain.Entities.ValuesObjects;

public sealed class AttachmentOwnerType
{
    public AttachmentOwnerTypeEnum Value { get; }

    private AttachmentOwnerType(AttachmentOwnerTypeEnum value)
    {
        Value = value;
    }

    public static AttachmentOwnerType Of(int value)
    {
        return !Enum.IsDefined(typeof(AttachmentOwnerTypeEnum), value)
            ? throw new ArgumentException($"Invalid owner type value: {value}")
            : new AttachmentOwnerType((AttachmentOwnerTypeEnum)value);
    }

    public static Result<AttachmentOwnerType> FromString(string ownerType)
    {
        if (string.IsNullOrWhiteSpace(ownerType))
        {
            return Result<AttachmentOwnerType>.Failure("Owner type cannot be null or empty");
        }

        if (Enum.TryParse<AttachmentOwnerTypeEnum>(ownerType, ignoreCase: true, out var enumValue))
        {
            if (Enum.IsDefined(typeof(AttachmentOwnerTypeEnum), enumValue))
            {
                return Result<AttachmentOwnerType>.Success(new AttachmentOwnerType(enumValue));
            }
        }

        var validValues = string.Join(", ", Enum.GetNames(typeof(AttachmentOwnerTypeEnum)));
        return Result<AttachmentOwnerType>.Failure(
            $"Invalid owner type: '{ownerType}'. Valid values are: {validValues}");
    }

    public static Result<AttachmentOwnerType> FromInt(int ownerTypeId)
    {
        if (!Enum.IsDefined(typeof(AttachmentOwnerTypeEnum), ownerTypeId))
        {
            var validValues = string.Join(", ",
                Enum.GetValues<AttachmentOwnerTypeEnum>().Select(v => $"{(int)v} ({v})"));
            return Result<AttachmentOwnerType>.Failure(
                $"Invalid owner type ID: {ownerTypeId}. Valid values are: {validValues}");
        }

        return Result<AttachmentOwnerType>.Success(new AttachmentOwnerType((AttachmentOwnerTypeEnum)ownerTypeId));
    }

    public static AttachmentOwnerType FromEnum(AttachmentOwnerTypeEnum enumValue)
    {
        return new AttachmentOwnerType(enumValue);
    }

    public string ToStringValue() => Value.ToString();

    public int ToInt() => (int)Value;

    public bool IsQuestion() => Value == AttachmentOwnerTypeEnum.Question;

    public bool IsAnswer() => Value == AttachmentOwnerTypeEnum.Answer;

    public static implicit operator AttachmentOwnerTypeEnum(AttachmentOwnerType ownerType) => ownerType.Value;

    public override string ToString() => Value.ToString();

    public override bool Equals(object? obj)
    {
        if (obj is AttachmentOwnerType other)
            return Value == other.Value;
        if (obj is AttachmentOwnerTypeEnum enumValue)
            return Value == enumValue;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();
}