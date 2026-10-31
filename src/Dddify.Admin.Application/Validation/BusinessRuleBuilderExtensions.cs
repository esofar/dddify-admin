namespace Dddify.Admin.Application.Validation;

/// <summary>
/// 可复用的业务字段格式校验扩展。
/// </summary>
public static class BusinessRuleBuilderExtensions
{
    /// <summary>
    /// 校验指定长度的数字验证码，默认长度为六位。
    /// </summary>
    /// <remarks>
    /// 只允许 ASCII 数字 0–9，允许前导零，不修剪输入。
    /// null 值不在此处判定为无效，必填字段应同时使用 NotEmpty。
    /// 本规则只校验格式，不判断验证码是否正确、过期或已使用。
    /// </remarks>
    /// <typeparam name="T">待校验对象类型。</typeparam>
    /// <param name="ruleBuilder">字段规则构建器。</param>
    /// <param name="length">验证码位数，必须大于零。</param>
    /// <returns>可继续追加规则、错误消息和条件的规则构建器。</returns>
    public static IRuleBuilderOptions<T, string?> VerificationCode<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int length = 6)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

        return ruleBuilder.Matches($@"\A[0-9]{{{length}}}\z");
    }
}
