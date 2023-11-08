using Dddify.Admin.Application.Commands.Tokens;
using Dddify.Admin.Application.Dtos.Tokens;
using Dddify.Admin.Host.Models.Tokens;

namespace Dddify.Admin.Host.Controllers;

[Route("api/v1/tokens")]
public class TokenController : BaseController
{
    /// <summary>
    /// ���ɷ������ƣ��˺����룩
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
    /// ���ɷ������ƣ��ֻ�����֤�룩
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
    /// ˢ�����ƽ����·�������
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
    /// ����ˢ������
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task RevokeTokenAsync([FromBody] RevokeTokenRequest request)
    {
        await SendAsync(new RevokeTokenCommand(request.RefreshToken));
    }
}