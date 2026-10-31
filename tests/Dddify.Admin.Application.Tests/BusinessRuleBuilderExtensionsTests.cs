using Dddify.Admin.Application.Validation;
using FluentValidation;

namespace Dddify.Admin.Application.Tests;

public class BusinessRuleBuilderExtensionsTests
{
    [Theory]
    [InlineData("123456", true)]
    [InlineData("000123", true)]
    [InlineData("000000", true)]
    [InlineData("999999", true)]
    [InlineData("", false)]
    [InlineData("12345", false)]
    [InlineData("1234567", false)]
    [InlineData("abcdef", false)]
    [InlineData("12345a", false)]
    [InlineData("１２３４５６", false)]
    [InlineData("١٢٣٤٥٦", false)]
    [InlineData(" 123456", false)]
    [InlineData("123456 ", false)]
    [InlineData("123456\n", false)]
    [InlineData("123456\r\n", false)]
    [InlineData("123\n45", false)]
    [InlineData("+12345", false)]
    public void VerificationCode_Requires_Exactly_Six_Ascii_Digits(string code, bool valid)
    {
        var validator = new InlineValidator<CodeInput>();
        validator.RuleFor(input => input.Code)
            .VerificationCode();

        Assert.Equal(valid, validator.Validate(new CodeInput(code)).IsValid);
    }

    [Fact]
    public void VerificationCode_Leaves_Requiredness_To_NotEmpty()
    {
        var optional = new InlineValidator<CodeInput>();
        optional.RuleFor(input => input.Code)
            .VerificationCode();

        var required = new InlineValidator<CodeInput>();
        required.RuleFor(input => input.Code)
            .NotEmpty()
            .VerificationCode();

        Assert.True(optional.Validate(new CodeInput(null)).IsValid);
        var error = Assert.Single(required.Validate(new CodeInput(null)).Errors);
        Assert.Equal("NotEmptyValidator", error.ErrorCode);
    }

    [Theory]
    [InlineData(4, "0123", true)]
    [InlineData(4, "012345", false)]
    [InlineData(8, "00001234", true)]
    [InlineData(8, "012345", false)]
    public void VerificationCode_Supports_An_Explicit_Length(int length, string code, bool valid)
    {
        var validator = new InlineValidator<CodeInput>();
        validator.RuleFor(input => input.Code)
            .VerificationCode(length);

        Assert.Equal(valid, validator.Validate(new CodeInput(code)).IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void VerificationCode_Rejects_Invalid_Length_When_Defining_Rules(int length)
    {
        var validator = new InlineValidator<CodeInput>();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            validator.RuleFor(input => input.Code).VerificationCode(length));
    }

    [Fact]
    public void VerificationCode_Preserves_The_BuiltIn_Error_Code_And_Field_Name()
    {
        var validator = new InlineValidator<CodeInput>();
        validator.RuleFor(input => input.Code)
            .VerificationCode();

        var error = Assert.Single(validator.Validate(new CodeInput("123456\n")).Errors);

        Assert.Equal("RegularExpressionValidator", error.ErrorCode);
        Assert.Equal(nameof(CodeInput.Code), error.PropertyName);
        Assert.False(string.IsNullOrWhiteSpace(error.ErrorMessage));
    }

    [Fact]
    public void VerificationCode_Allows_The_Caller_To_Customize_The_Error()
    {
        var validator = new InlineValidator<CodeInput>();
        validator.RuleFor(input => input.Code)
            .VerificationCode()
            .WithErrorCode("invalid_verification_code_format")
            .WithMessage("验证码必须为六位数字。");

        var error = Assert.Single(validator.Validate(new CodeInput("invalid")).Errors);

        Assert.Equal("invalid_verification_code_format", error.ErrorCode);
        Assert.Equal("验证码必须为六位数字。", error.ErrorMessage);
    }

    private sealed record CodeInput(string? Code);
}
