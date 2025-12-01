using Forum.Domain.Entities.ValuesObjects;
using Forum.Domain.Enums;

namespace Forum.UnitTests.Domain.Enums;

public class AttachmentOwnerTypeUnitTest
{
    [Theory]
    [InlineData("Question", AttachmentOwnerTypeEnum.Question)]
    [InlineData("question", AttachmentOwnerTypeEnum.Question)]
    [InlineData("QUESTION", AttachmentOwnerTypeEnum.Question)]
    [InlineData("Answer", AttachmentOwnerTypeEnum.Answer)]
    [InlineData("answer", AttachmentOwnerTypeEnum.Answer)]
    [InlineData("ANSWER", AttachmentOwnerTypeEnum.Answer)]
    public void FromString_WithValidString_ShouldReturnSuccess(string input, AttachmentOwnerTypeEnum expected)
    {
           var result = AttachmentOwnerType.FromString(input);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expected, result.Value.Value);
    }

    [Theory]
    [InlineData("Invalid")]
    [InlineData("Test")]
    [InlineData("123")]
    [InlineData("question answer")]
    public void FromString_WithInvalidString_ShouldReturnFailure(string input)
    {
        var result = AttachmentOwnerType.FromString(input);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("Invalid owner type", result.Error);
        Assert.Contains(input, result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void FromString_WithEmptyOrWhitespace_ShouldReturnFailure(string input)
    {
        var result = AttachmentOwnerType.FromString(input);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("cannot be null or empty", result.Error);
    }

    [Fact]
    public void FromString_WithNull_ShouldReturnFailure()
    {
        var result = AttachmentOwnerType.FromString(null!);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("cannot be null or empty", result.Error);
    }

    [Theory]
    [InlineData(1, AttachmentOwnerTypeEnum.Question)]
    [InlineData(2, AttachmentOwnerTypeEnum.Answer)]
    public void FromInt_WithValidId_ShouldReturnSuccess(int input, AttachmentOwnerTypeEnum expected)
    {
        var result = AttachmentOwnerType.FromInt(input);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expected, result.Value.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(99)]
    [InlineData(-1)]
    public void FromInt_WithInvalidId_ShouldReturnFailure(int input)
    {
        var result = AttachmentOwnerType.FromInt(input);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Contains("Invalid owner type ID", result.Error);
        Assert.Contains(input.ToString(), result.Error);
    }

    [Theory]
    [InlineData(AttachmentOwnerTypeEnum.Question)]
    [InlineData(AttachmentOwnerTypeEnum.Answer)]
    public void FromEnum_ShouldCreateValidInstance(AttachmentOwnerTypeEnum enumValue)
    {
        var ownerType = AttachmentOwnerType.FromEnum(enumValue);

        Assert.NotNull(ownerType);
        Assert.Equal(enumValue, ownerType.Value);
    }

    [Fact]
    public void IsQuestion_WhenQuestion_ShouldReturnTrue()
    {
        var ownerType = AttachmentOwnerType.FromEnum(AttachmentOwnerTypeEnum.Question);

        Assert.True(ownerType.IsQuestion());
        Assert.False(ownerType.IsAnswer());
    }

    [Fact]
    public void IsAnswer_WhenAnswer_ShouldReturnTrue()
    {
        var ownerType = AttachmentOwnerType.FromEnum(AttachmentOwnerTypeEnum.Answer);

        Assert.True(ownerType.IsAnswer());
        Assert.False(ownerType.IsQuestion());
    }

    [Theory]
    [InlineData(AttachmentOwnerTypeEnum.Question, "Question")]
    [InlineData(AttachmentOwnerTypeEnum.Answer, "Answer")]
    public void ToStringValue_ShouldReturnEnumName(AttachmentOwnerTypeEnum enumValue, string expected)
    {
        var ownerType = AttachmentOwnerType.FromEnum(enumValue);

        var result = ownerType.ToStringValue();

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(AttachmentOwnerTypeEnum.Question, 1)]
    [InlineData(AttachmentOwnerTypeEnum.Answer, 2)]
    public void ToInt_ShouldReturnEnumValue(AttachmentOwnerTypeEnum enumValue, int expected)
    {
        var ownerType = AttachmentOwnerType.FromEnum(enumValue);

        var result = ownerType.ToInt();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertToEnum()
    {
        var ownerType = AttachmentOwnerType.FromEnum(AttachmentOwnerTypeEnum.Question);

        AttachmentOwnerTypeEnum enumValue = ownerType;

        Assert.Equal(AttachmentOwnerTypeEnum.Question, enumValue);
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        var ownerType1 = AttachmentOwnerType.FromEnum(AttachmentOwnerTypeEnum.Question);
        var ownerType2 = AttachmentOwnerType.FromEnum(AttachmentOwnerTypeEnum.Question);

        Assert.True(ownerType1.Equals(ownerType2));
        Assert.Equal(ownerType1.GetHashCode(), ownerType2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldReturnFalse()
    {
        var ownerType1 = AttachmentOwnerType.FromEnum(AttachmentOwnerTypeEnum.Question);
        var ownerType2 = AttachmentOwnerType.FromEnum(AttachmentOwnerTypeEnum.Answer);

        Assert.False(ownerType1.Equals(ownerType2));
    }

    [Fact]
    public void Equals_WithEnum_ShouldReturnTrue()
    {
        var ownerType = AttachmentOwnerType.FromEnum(AttachmentOwnerTypeEnum.Question);

        Assert.True(ownerType.Equals(AttachmentOwnerTypeEnum.Question));
    }

    [Fact]
    public void ToString_ShouldReturnEnumName()
    {
        var ownerType = AttachmentOwnerType.FromEnum(AttachmentOwnerTypeEnum.Question);

        var result = ownerType.ToString();

        Assert.Equal("Question", result);
    }

    [Fact]
    public void FromString_ErrorMessage_ShouldIncludeValidValues()
    {
        var result = AttachmentOwnerType.FromString("Invalid");

        Assert.False(result.IsSuccess);
        Assert.Contains("Question", result.Error);
        Assert.Contains("Answer", result.Error);
    }

    [Fact]
    public void FromInt_ErrorMessage_ShouldIncludeValidValues()
    {
        var result = AttachmentOwnerType.FromInt(99);

        Assert.False(result.IsSuccess);
        Assert.Contains("1 (Question)", result.Error);
        Assert.Contains("2 (Answer)", result.Error);
    }
}

