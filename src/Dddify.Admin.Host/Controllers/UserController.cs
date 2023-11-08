using Dddify.Admin.Application.Commands.Users;
using Dddify.Admin.Application.Dtos.Dictionaries;
using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Application.Queries.Dictionaries;
using Dddify.Admin.Application.Queries.Users;
using Dddify.Admin.Host.Models.Users;

namespace Dddify.Admin.Host.Controllers;

[Route("api/v1/users")]
public class UserController : BaseController
{
    /// <summary>
    /// ��ȡ��ǰ�û���Ϣ
    /// </summary>
    /// <returns></returns>
    [HttpGet("current")]
    public async Task<UserProfileDto> GetCurrentUserAsync()
    {
        return await SendAsync(new GetUserProfileQuery());
    }

    /// <summary>
    /// �û��б�
    /// </summary>
    /// <param name="current">��ǰҳ</param>
    /// <param name="pageSize">ҳ����</param>
    /// <param name="fullName">����</param>
    /// <param name="gender">�Ա�</param>
    /// <param name="organizationId">����ID</param>
    /// <param name="status">�˺�״̬</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(IApiResult<IPagedResult<UserDto>>))]
    [HttpGet]
    public async Task<IPagedResult<UserDto>> GetAsync(int current, int pageSize, string? fullName, UserGender? gender, Guid? organizationId, UserStatus? status)
    {
        return await SendAsync(new GetUsersQuery(
            current,
            pageSize,
            fullName,
            gender,
            organizationId,
            status));
    }

    /// <summary>
    /// ��ȡ�û�
    /// </summary>
    /// <param name="id">�û�ID</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<UserDto> GetAsync(Guid id)
    {
        return await SendAsync(new GetUserByIdQuery(id));
    }

    /// <summary>
    /// �½��û�
    /// </summary>
    /// <param name="request">����ģ��</param>
    /// <returns></returns>
    [HttpPost]
    public async Task CreateAsync([FromBody] CreateUserRequest request)
    {
        await SendAsync(new CreateUserCommand(
            request.FullName,
            request.NickName,
            request.Gender,
            request.BirthDate,
            request.Email,
            request.PhoneNumber,
            request.OrganizationId,
            request.Type));
    }

    /// <summary>
    /// �޸��û�
    /// </summary>
    /// <param name="id">�û�ID</param>
    /// <param name="request">����ģ��</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, [FromBody] UpdateUserRequest request)
    {
        await SendAsync(new UpdateUserCommand(
            id,
            request.FullName,
            request.NickName,
            request.Gender,
            request.BirthDate,
            request.Email,
            request.PhoneNumber,
            request.OrganizationId,
            request.Type,
            request.ConcurrencyStamp));
    }

    /// <summary>
    /// ɾ���û�
    /// </summary>
    /// <param name="id">�û�ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await SendAsync(new DeleteUserCommand(id));
    }

    /// <summary>
    /// ��Ա�����б�
    /// </summary>
    /// <returns></returns>
    [HttpGet("types")]
    public async Task<IEnumerable<DictionaryItemDto>> GetTypesAsync()
    {
        return await SendAsync(new GetItemsByDictionaryCodeQuery(DictionaryCodes.UserType));
    }
}