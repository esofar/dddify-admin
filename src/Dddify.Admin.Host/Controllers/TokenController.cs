using Dddify.Admin.Application.Commands.Tokens;
using Dddify.Admin.Application.Dtos.Tokens;
using Dddify.Admin.Host.Models.Tokens;

namespace Dddify.Admin.Host.Controllers;

[Route("api/v1/tokens")]
public class TokenController : BaseController
{
    /// <summary>
    /// 生成访问令牌（账号密码）
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("email")]
    public async Task<TokenDto> GenerateTokenByEmailAsync([FromBody] GenerateTokenByEmailRequest request)
    {
        return await SendAsync(new GenerateTokenByEmailCommand(request.Email, request.Password));
    }

    /// <summary>
    /// 生成访问令牌（手机号验证码）
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("phone")]
    public async Task<TokenDto> GenerateTokenByPhoneAsync([FromBody] GenerateTokenByPhoneRequest request)
    {
        return await SendAsync(new GenerateTokenByPhoneCommand(request.PhoneNumber, request.Captcha));
    }

    /// <summary>
    /// 刷新令牌交换新访问令牌
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPut]
    public async Task<TokenDto> ExchangeTokenAsync([FromBody] ExchangeTokenRequest request)
    {
        return await SendAsync(new ExchangeTokenCommand(request.AccessToken, request.RefreshToken));
    }

    /// <summary>
    /// 撤销刷新令牌
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task RevokeTokenAsync([FromBody] RevokeTokenRequest request)
    {
        await SendAsync(new RevokeTokenCommand(request.RefreshToken));
    }
}